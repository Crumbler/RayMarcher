using System;
using RayMarchLib;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

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

                var progressBar = new ConsoleProgressBar(marcher.TotalProgress);

                progressBar.ShowProgress(0);

                Task t = marcher.CalculateFrameAsync(4);

                while (marcher.CurrProgress < marcher.TotalProgress)
                {
                    progressBar.ShowProgress(marcher.CurrProgress);
                    Thread.Sleep(100);
                }

                t.Wait();

                progressBar.ShowProgress(marcher.CurrProgress);
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

    public class ConsoleProgressBar
    {
        private const ConsoleColor ForeColor = ConsoleColor.Green;
        private const ConsoleColor BkColor = ConsoleColor.Gray;
        private const int DefaultWidthOfBar = 64;
        private const int TextMarginLeft = 3;

        private readonly int _total;
        private readonly int _widthOfBar;

        public ConsoleProgressBar(int total, int widthOfBar = DefaultWidthOfBar)
        {
            _total = total;
            _widthOfBar = widthOfBar;
        }

        private bool _intited;
        public void Init()
        {
            _lastPosition = 0;

            //Draw empty progress bar
            Console.CursorVisible = false;
            Console.CursorLeft = 0;
            Console.Write("["); //start
            Console.CursorLeft = _widthOfBar;
            Console.Write("]"); //end
            Console.CursorLeft = 1;

            //Draw background bar
            for (var position = 1; position < _widthOfBar; position++) //Skip the first position which is "[".
            {
                Console.BackgroundColor = BkColor;
                Console.CursorLeft = position;
                Console.Write(" ");
            }
        }

        public void ShowProgress(int currentCount)
        {
            if (!_intited)
            {
                Init();
                _intited = true;
            }
            DrawTextProgressBar(currentCount);
        }

        private int _lastPosition;

        public void DrawTextProgressBar(int currentCount)
        {
            //Draw current chunk.
            var position = currentCount * _widthOfBar / _total;
            if (position != _lastPosition)
            {
                Console.BackgroundColor = ForeColor;
                //Console.CursorLeft = position >= _widthOfBar ? _widthOfBar - 1 : position;
                Console.CursorLeft = _lastPosition;

                for (int i = _lastPosition; i < position; ++i)
                {
                    Console.Write(" ");
                }
                _lastPosition = position;
            }

            //Draw totals
            Console.CursorLeft = _widthOfBar + TextMarginLeft;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(currentCount + " of " + _total + "    "); //blanks at the end remove any excess
        }
    }
}