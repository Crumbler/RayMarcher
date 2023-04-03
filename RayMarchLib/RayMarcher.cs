using System;
using System.Drawing;
using System.Numerics;
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
                    rowEnd = (i + 1) * rowsPerThread;

                if (i == threads - 1)
                {
                    rowEnd = Bitmap.Height - 1;
                }

                tasks[i] = Task.Run(() => CalculateRow(rowStart, rowEnd));
            }

            Task.WaitAll(tasks);
        }

        private void CalculateRow(int rowStart, int rowEnd)
        {
            for (int i = rowStart; i <= rowEnd; ++i)
            {
                for (int j = 0; j < Bitmap.Width; ++j)
                {
                    CalculatePixel(i, j);
                }
            }
        }

        private Vector3 GetRayDir(int y, int x)
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

        private Vector3 GetPointColor(Vector3 v, Material m, Vector3 rayDir)
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
                float attenuation;
                bool inShadow;

                if (l.LightType == LightType.Directional)
                {
                    attenuation = 1f;
                    posToLight = -l.Direction;

                    var marchRes = March(v + n * Scene.Eps * 2f, posToLight);
                    inShadow = marchRes.distToObject < Scene.Eps;
                }
                else // Point light
                {
                    float dist = (l.Position - v).Length();

                    attenuation = 1f /
                        (l.Attenuation.X +
                        l.Attenuation.Y * dist +
                        l.Attenuation.Z * dist * dist);

                    posToLight = Vector3.Normalize(l.Position - v);

                    var marchRes = March(v + n * Scene.Eps * 2f, posToLight);
                    inShadow = marchRes.distance < dist - Scene.Eps;
                }

                float shadowFactor = inShadow ? Scene.ShadowFactor : 1f;

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
                        MathF.Pow(shineFactor, m.Specular) * shadowFactor;
                }
            }

            ambient *= m.Ambient;
            diffuse *= m.Diffuse;

            //return (n + Vector3.One) * 0.5f;

            return (ambient + diffuse + specular) * m.Color;
        }

        private MarchResult March(Vector3 origin, Vector3 direction)
        {
            Vector3 rayDir = direction;

            Vector3 rayOrigin = origin, pos = rayOrigin;
            float t = 0.0f, h = 0.0f;

            for (int i = 0; i < Scene.MaxIterations; ++i)
            {
                pos = rayOrigin + rayDir * t;

                h = Map(pos);

                if (h < Scene.Eps || h > Scene.MaxDist)
                {
                    break;
                }

                t += h;
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

        private void CalculatePixel(int y, int x)
        {
            MarchResult res = March(RayOrigin, GetRayDir(y, x));

            Vector3 c;

            if (res.distToObject < Scene.Eps)
            {
                HitResult hit = GetHit(res.position);

                Material m = hit.material ?? Material.Default;

                c = GetPointColor(res.position, m, res.direction);
            }
            else
            {
                c = Material.Background.Color;
            }

            // Gamma correction
            c = Utils.Pow(c, 1f / 2.2f);

            Bitmap.SetPixel(x, y, c.ToColor());
        }
    }
}
