
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
            var bmp = new DirectBitmap(1280, 720);

            var scene = new Scene();

            var s1 = new Sphere()
            {
                Radius = 0.5f,
                Position = new Vector3(0, -1, -3)
            };

            scene.Objects.Add(s1);

            var marcher = new RayMarcher(scene, bmp);

            marcher.CalculateFrame(4);

            marcher.Bitmap.Bitmap.Save("img.png", ImageFormat.Png);
        }
    }
}