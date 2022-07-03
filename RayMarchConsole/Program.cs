
using System;
using RayMarchLib;
using System.Drawing;

namespace RayMarchConsole
{
    public static class Program
    {
        public static void Main()
        {
            var bmp = new DirectBitmap(800, 800);

            RayMarcher.CalculateFrame(new Scene(), bmp);

            bmp.Bitmap.Save("img.png", System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}