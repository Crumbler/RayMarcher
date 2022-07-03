using System;
using System.Drawing;
using System.Numerics;
using System.Threading.Tasks;

namespace RayMarchLib
{
    public static class RayMarcher
    {
        private static Matrix4x4 prMat;

        public static void CalculateFrame(Scene scene, DirectBitmap bitmap, int threads = 1)
        {
            prMat = Matrix4x4.CreatePerspectiveFieldOfView(MathF.PI / 2.0f, 1.0f, 0.1f, 1000.0f);

            int rowsPerThread = bitmap.Height / threads;

            var tasks = new Task[threads];

            for (int i = 0; i < threads; ++i)
            {
                int rowStart = i * rowsPerThread,
                    rowEnd = (i + 1) * rowsPerThread;

                if (i == threads - 1)
                {
                    rowEnd = bitmap.Height - 1;
                }

                tasks[i] = Task.Run(() => CalculateRow(bitmap, rowStart, rowEnd));
            }

            Task.WaitAll(tasks);
        }

        private static void CalculateRow(DirectBitmap bmp, int rowStart, int rowEnd)
        {
            for (int i = rowStart; i < rowEnd; ++i)
            {
                for (int j = 0; j < bmp.Width; ++j)
                {
                    CalculatePixel(bmp, i, j);
                }
            }
        }

        private static Vector3 GetRayDir(DirectBitmap bmp, int y, int x)
        {
            var rayDir = new Vector3((x * 2.0f - bmp.Width) / bmp.Width,
                                     (y * 2.0f - bmp.Height) / bmp.Height,
                                     -1);

            rayDir = Vector3.Transform(rayDir, prMat);
            rayDir = Vector3.Normalize(rayDir);

            return rayDir;
        }

        private static float Map(Vector3 v)
        {
            return Vector3.Distance(v, new(0, 0, 5)) - 0.5f;
        }

        private static void CalculatePixel(DirectBitmap bmp, int y, int x)
        {
            var c = Color.Black;

            Vector3 rayDir = GetRayDir(bmp, y, x);

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

            bmp.SetPixel(x, y, c);
        }
    }
}
