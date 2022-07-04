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

        private Matrix4x4 prMat;

        public RayMarcher(Scene scene, DirectBitmap bitmap)
        {
            Scene = scene;
            Bitmap = bitmap;
        }

        public RayMarcher()
        {
            Scene = new Scene();
            Bitmap = new DirectBitmap(100, 100);
        }

        public void CalculateFrame(int threads = 1)
        {
            prMat = Matrix4x4.CreatePerspectiveFieldOfView(MathF.PI / 2.0f, 1.0f, 0.1f, 1000.0f);

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
            for (int i = rowStart; i < rowEnd; ++i)
            {
                for (int j = 0; j < Bitmap.Width; ++j)
                {
                    CalculatePixel(i, j);
                }
            }
        }

        private Vector3 GetRayDir(int y, int x)
        {
            var rayDir = new Vector3((x * 2.0f - Bitmap.Width) / Bitmap.Width,
                                     (y * 2.0f - Bitmap.Height) / Bitmap.Height,
                                     -1);

            rayDir = Vector3.Transform(rayDir, prMat);
            rayDir = Vector3.Normalize(rayDir);

            return rayDir;
        }

        private float Map(Vector3 v)
        {
            return Vector3.Distance(v, new(0, 0, 5)) - 0.5f;
        }

        private void CalculatePixel(int y, int x)
        {
            var c = Color.Black;

            Vector3 rayDir = GetRayDir(y, x);

            Vector3 rayOrigin = Vector3.Zero, pos;
            float t = 0.0f, h = 0.0f;

            for (int i = 0; i < 50; ++i)
            {
                pos = rayOrigin + rayDir * t;

                h = Map(pos);

                if (h < 0.001f || h > 10.0f)
                {
                    break;
                }

                t += h;
            }

            if (h < 0.001f)
            {
                c = Color.White;
            }

            Bitmap.SetPixel(x, y, c);
        }
    }
}
