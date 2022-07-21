
using System;
using RayMarchLib;
using System.Drawing.Imaging;
using System.Numerics;

namespace RayMarchConsole
{
    public static class Program
    {
        public static void Main()
        {
            var bmp = new DirectBitmap(800, 800);

            var scene = new Scene();

            var s1 = new Sphere()
            {
                Radius = 0.5f,
                Position = new Vector3(0, 0, 5)
            };

            scene.Objects.Add(s1);

            var marcher = new RayMarcher(scene, bmp);

            marcher.CalculateFrame(4);

            marcher.Bitmap.Bitmap.Save("img.png", ImageFormat.Png);
        }
    }
}