using System;
using System.Drawing;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace RayMarchLib
{
    public class RayMarcher
    {
        public Scene Scene { get; set; }
        public DirectBitmap Bitmap { get; set; }
        public Bitmap ActualBitmap { get => Bitmap.Bitmap; }

        private Matrix4x4 projMatInv, viewMatInv;
        private Vector3 RayOrigin { get; set; }
        private int currProgress;
        public int CurrProgress
        {
            get
            {
                return currProgress;
            }
        }
        public int TotalProgress
        {
            get
            {
                return Scene.ImageHeight;
            }
        }

        public RayMarcher(Scene scene, DirectBitmap bitmap)
        {
            Scene = scene;
            Bitmap = bitmap;
        }

        public RayMarcher(Scene scene)
        {
            Scene = scene;
            Bitmap = new DirectBitmap(Scene.ImageWidth, Scene.ImageHeight);
        }

        public RayMarcher()
        {
            Scene = new Scene();
            Bitmap = new DirectBitmap(Scene.ImageWidth, Scene.ImageHeight);
        }

        public void CalculateFrame(int threads = 1)
        {
            CalcFrame(threads).Wait();
        }

        private Task CalcFrame(int threads = 1)
        {
            currProgress = 0;

            if (Scene.ImageWidth != Bitmap.Width || Scene.ImageHeight != Bitmap.Height)
            {
                Bitmap.Dispose();
                Bitmap = new DirectBitmap(Scene.ImageWidth, Scene.ImageHeight);
            }

            Scene.PreCalculate();

            float aspectRatio = (float)Bitmap.Width / Bitmap.Height;
            const float nearPlane = 0.1f, farPlane = 1000f;

            viewMatInv = Matrix4x4.CreateLookAt(Scene.Camera.Position, Scene.Camera.Target, Vector3.UnitY);
            Matrix4x4.Invert(viewMatInv, out viewMatInv);

            RayOrigin = Vector3.Transform(Vector3.Zero, viewMatInv);

            projMatInv = Matrix4x4.CreatePerspectiveFieldOfView(Scene.Fov, aspectRatio, nearPlane, farPlane);
            Matrix4x4.Invert(projMatInv, out projMatInv);

            int rowsPerThread = Bitmap.Height / threads;

            var tasks = new Task[threads];

            for (int i = 0; i < threads; ++i)
            {
                int rowStart = i * rowsPerThread,
                    rowEnd = Math.Min((i + 1) * rowsPerThread, Bitmap.Height);

                tasks[i] = Task.Run(CalculateRows);
            }

            return Task.WhenAll(tasks);
        }

        public async Task CalculateFrameAsync(int threads = 1)
        {
            await CalcFrame(threads);
        }

        private void CalculateRows()
        {
            while (currProgress < Bitmap.Height)
            {
                int row = Interlocked.Increment(ref currProgress) - 1;
                if (row >= Bitmap.Height)
                {
                    break;
                }

                for (int j = 0; j < Bitmap.Width; ++j)
                {
                    CalculatePixel(row, j);
                }
            }
        }

        private Vector3 GetRayDir(float y, float x)
        {
            int bWidth = Bitmap.Width - 1,
                bHeight = Bitmap.Height - 1;

            var rayDir = new Vector3((x * 2.0f - bWidth) / bWidth,
                                     (bHeight - y * 2.0f) / bHeight,
                                     0.0f);

            rayDir = Vector3.Transform(rayDir, projMatInv);
            rayDir.Z = -1.0f;
            rayDir = Vector3.TransformNormal(rayDir, viewMatInv);
            rayDir = Vector3.Normalize(rayDir);

            return rayDir;
        }

        private float Map(Vector3 v)
        {
            float minDist = float.PositiveInfinity;
            
            for (int i = 0; i < Scene.Objects.Count; ++i)
            {
                float dist = Scene.Objects[i].Map(v);
                if (minDist > dist)
                {
                    minDist = dist;
                }
            }
            
            return minDist;
        }

        private Vector3 GetNormal(Vector3 v)
        {
            const float h = 0.0001f;

            var res = new Vector3(1, -1, -1) * Map(v + new Vector3(1, -1, -1) * h) +
                new Vector3(-1, -1, 1) * Map(v + new Vector3(-1, -1, 1) * h) +
                new Vector3(-1, 1, -1) * Map(v + new Vector3(-1, 1, -1) * h) +
                Vector3.One * Map(v + Vector3.One * h);

            return Vector3.Normalize(res);
        }

        private HitResult GetHit(Vector3 v)
        {
            float minDist = float.PositiveInfinity;
            RMObject hitObj = null;

            for (int i = 0; i < Scene.Objects.Count; ++i)
            {
                float dist = Scene.Objects[i].Map(v);
                if (minDist > dist)
                {
                    minDist = dist;
                    hitObj = Scene.Objects[i];
                }
            }

            return hitObj.MapHit(v);
        }

        private Vector3 GetColorAtPoint(Vector3 v, Material m, Vector3 rayDir)
        {
            if (Scene.LightingType == LightingType.None)
            {
                return m.Color;
            }

            Vector3 n = GetNormal(v);
            Vector3 ambient = Vector3.Zero, diffuse = Vector3.Zero,
                    specular = Vector3.Zero;

            for (int i = 0; i < Scene.Lights.Count; ++i)
            {
                Light l = Scene.Lights[i];

                Vector3 posToLight;
                float attenuation, distToLight;

                if (l.LightType == LightType.Directional)
                {
                    attenuation = 1f;
                    posToLight = -l.Direction;
                    distToLight = Scene.MaxDist;
                }
                else // Point light
                {
                    float dist = (l.Position - v).Length();

                    attenuation = 1f /
                        (l.Attenuation.X +
                        l.Attenuation.Y * dist +
                        l.Attenuation.Z * dist * dist);

                    posToLight = Vector3.Normalize(l.Position - v);
                    distToLight = dist;
                }

                float shadowFactor;
                if (Scene.SoftShadows)
                {
                    MarchShadow(v + n * Scene.Eps * 2f, posToLight, distToLight, out shadowFactor);
                }
                else
                {
                    var marchRes = March(v + n * Scene.Eps * 2f, posToLight, distToLight);
                    if (l.LightType == LightType.Directional)
                    {
                        shadowFactor = marchRes.distToObject < Scene.Eps ? Scene.ShadowFactor : 1f;
                    }
                    else
                    {
                        shadowFactor = marchRes.distance < distToLight - Scene.Eps ? Scene.ShadowFactor : 1f;
                    }
                }

                float diff = MathF.Max(0f, Vector3.Dot(posToLight, n));

                ambient += l.Color * l.Intensity * attenuation;

                diffuse += l.Color * l.Intensity * diff * attenuation * shadowFactor;

                if (diff > 0f)
                {
                    float shineFactor;

                    if (Scene.LightingType == LightingType.Phong)
                    {
                        var reflDir = Vector3.Reflect(posToLight, n);
                        shineFactor = Vector3.Dot(reflDir, rayDir);
                    }
                    else // Blinn-Phong
                    {
                        var halfVec = Vector3.Normalize(posToLight - rayDir);
                        shineFactor = Vector3.Dot(n, halfVec);
                    }

                    shineFactor = MathF.Max(0f, shineFactor);

                    specular += l.Color * attenuation * l.Intensity * 
                        MathF.Pow(shineFactor, m.Shininess) * m.Specular * shadowFactor;
                }
            }

            ambient *= m.Ambient;
            diffuse *= m.Diffuse;

            return (ambient + diffuse) * m.Color + specular;
        }

        private MarchResult MarchShadowSphere(Vector3 origin, Vector3 direction, float maxDist, out float res)
        {
            Vector3 rayDir = direction;
            Vector3 rayOrigin = origin, pos = rayOrigin;

            float ph = 1e10f;
            float t = 0.1f, sRes = 1f, h = float.PositiveInfinity;

            for (int i = 0; i < Scene.MaxIterations && t < maxDist; ++i)
            {
                pos = rayOrigin + rayDir * t;

                h = Map(pos);

                if (h < Scene.Eps)
                {
                    res = Scene.ShadowFactor;
                    return new MarchResult()
                    {
                        direction = direction,
                        distance = t,
                        distToObject = h,
                        origin = origin,
                        position = pos
                    };
                }

                float y = h * h / (2f * ph);
                float d = MathF.Sqrt(h * h - y * y);

                float divisor = Scene.ShadowSoftness * d / (t - y).MaxZero();

                if (!float.IsNaN(divisor))
                {
                    sRes = MathF.Min(sRes, divisor);
                }

                ph = h;

                t = MathF.Min(t + h, maxDist);
            }

            res = Utils.Clamp(sRes, Scene.ShadowFactor, 1f);

            return new MarchResult()
            {
                direction = direction,
                distance = t,
                distToObject = h,
                origin = origin,
                position = pos
            };
        }

        private MarchResult MarchShadowFixed(Vector3 origin, Vector3 direction, float step, float maxDist, out float res)
        {
            Vector3 rayDir = direction, rayOrigin = origin,
                pos = rayOrigin;

            float ph = 1e10f;
            float t = 0.1f, sRes = 1f;
            float lastSign, sign = 1f, h = float.PositiveInfinity;

            for (int i = 0; i < Scene.MaxIterations && t < maxDist; ++i)
            {
                pos = rayOrigin + rayDir * t;

                h = Map(pos);

                if (h < Scene.Eps)
                {
                    res = Scene.ShadowFactor;
                    return new MarchResult()
                    {
                        direction = direction,
                        distance = t,
                        distToObject = h,
                        origin = origin,
                        position = pos
                    };
                }

                float y = h * h / (2f * ph);
                float d = MathF.Sqrt(h * h - y * y);

                float divisor = Scene.ShadowSoftness * d / (t - y).MaxZero();

                if (!float.IsNaN(divisor))
                {
                    sRes = MathF.Min(sRes, divisor);
                }

                ph = h;

                lastSign = sign;
                sign = Utils.Sign(h);

                if (sign != lastSign)
                {
                    step /= 2f;
                }

                t = MathF.Min(t + step * sign, maxDist);
            }

            res = Utils.Clamp(sRes, Scene.ShadowFactor, 1f);

            return new MarchResult()
            {
                direction = direction,
                distance = t,
                distToObject = h,
                origin = origin,
                position = pos
            };
        }

        private MarchResult MarchShadow(Vector3 origin, Vector3 direction, float maxDist, out float res)
        {
            if (Scene.MarchingAlgorithm == MarchingAlgorithm.SphereTracing)
            {
                return MarchShadowSphere(origin, direction, maxDist, out res);
            }
            else // Fixed step
            {
                return MarchShadowFixed(origin, direction, Scene.Step, maxDist, out res);
            }
        }

        private MarchResult MarchSphere(Vector3 origin, Vector3 direction, float maxDist)
        {
            Vector3 rayDir = direction;

            Vector3 rayOrigin = origin, pos = rayOrigin;
            float t = 0.0f, h = 0.0f;

            for (int i = 0; i < Scene.MaxIterations; ++i)
            {
                pos = rayOrigin + rayDir * t;

                h = Map(pos);

                if (h < Scene.Eps || t >= maxDist)
                {
                    break;
                }

                t = MathF.Min(t + h, maxDist);
            }

            return new MarchResult()
            {
                direction = direction,
                origin = origin,
                distance = t,
                distToObject = h,
                position = pos
            };
        }

        private MarchResult MarchFixed(Vector3 origin, Vector3 direction, float step, float maxDist)
        {
            Vector3 rayDir = direction;

            Vector3 rayOrigin = origin, pos = rayOrigin;
            float t = 0.0f, h = 0.0f, lastSign, sign = 1f;

            for (int i = 0; i < Scene.MaxIterations; ++i)
            {
                pos = rayOrigin + rayDir * t;

                h = Map(pos);

                if ((h < Scene.Eps && h > 0f) || t >= maxDist)
                {
                    break;
                }

                lastSign = sign;
                sign = Utils.Sign(h);

                if (sign != lastSign)
                {
                    step /= 2f;
                }
                
                t = MathF.Min(t + step * sign, maxDist);
            }

            return new MarchResult()
            {
                direction = direction,
                origin = origin,
                distance = t,
                distToObject = h,
                position = pos
            };
        }

        private MarchResult March(Vector3 origin, Vector3 direction, float maxDist)
        {
            if (Scene.MarchingAlgorithm == MarchingAlgorithm.SphereTracing)
            {
                return MarchSphere(origin, direction, maxDist);
            }
            else // Fixed step
            {
                return MarchFixed(origin, direction, Scene.Step, maxDist);
            }
        }

        private Vector3 GetPixelColor(float x, float y)
        {
            MarchResult res = March(RayOrigin, GetRayDir(y, x), Scene.MaxDist);

            if (res.distToObject < Scene.Eps)
            {
                HitResult hit = GetHit(res.position);

                Material m = hit.material ?? Material.Default;

                return GetColorAtPoint(res.position, m, res.direction);
            }
            else
            {
                return Material.Background.Color;
            }
        }

        private Vector3 CalculatePixelColor(float x, float y)
        {
            if (Scene.AntiAliasing)
            {
                Vector3 c1 = GetPixelColor(x + 3f / 8f, y - 1f / 8f),
                    c2 = GetPixelColor(x + 1f / 8f, y + 3f / 8f),
                    c3 = GetPixelColor(x - 3f / 8f, y + 1f / 8f),
                    c4 = GetPixelColor(x - 1f / 8f, y - 3f / 8f);

                return (c1 + c2 + c3 + c4) / 4f;
            }
            else
            {
                return GetPixelColor(x, y);
            }
        }

        private void CalculatePixel(int y, int x)
        {
            Vector3 c = CalculatePixelColor(x, y);

            // Gamma correction
            c = Utils.Pow(c, 1f / 2.2f);

            Bitmap.SetPixel(x, y, c.ToColor());
        }
    }
}
