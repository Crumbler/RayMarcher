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

        private void CalculatePixel(int y, int x)
        {
            Vector3 c, rayDir = GetRayDir(y, x);

            Vector3 rayOrigin = RayOrigin, pos = rayOrigin;
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

            if (h < Scene.Eps)
            {
                HitResult hit = GetHit(pos);

                Material m = hit.material ?? Material.Default;

                c = m.Color;
            }
            else
            {
                c = Material.Background.Color;
            }

            Bitmap.SetPixel(x, y, c.ToColor());
        }
    }
}
