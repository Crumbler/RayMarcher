
using System;
using RayMarchLib;
using System.Drawing.Imaging;

namespace RayMarchConsole
{
    public static class Program
    {
        public static void Main()
        {
            var bmp = new DirectBitmap(800, 800);

            var marcher = new RayMarcher(new Scene(), bmp);

            marcher.CalculateFrame();

            marcher.Bitmap.Bitmap.Save("img.png", ImageFormat.Png);
        }
    }
}