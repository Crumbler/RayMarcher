using System;
using RayMarchLib;
using System.Drawing.Imaging;
using System.IO;

namespace RayMarchConsole
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Exactly one argument must be specified.");
                Console.ReadKey();
                return;
            }

            string path = args[0];

            if (string.IsNullOrWhiteSpace(path))
            {
                Console.WriteLine("Empty path specified.");
                Console.ReadKey();
                return;
            }

            RayMarcher marcher;
            try
            {
                var scene = Scene.LoadFromFile(path);

                marcher = new RayMarcher(scene);

                marcher.CalculateFrame(4);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error:\n{e.Message}");
                Console.ReadKey();
                return;
            }

            string imgPath = Path.ChangeExtension(path, "png");
            marcher.Bitmap.Bitmap.Save(imgPath, ImageFormat.Png);

            Console.WriteLine("Rendering complete.");
            Console.ReadKey();
        }
    }
}