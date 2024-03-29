﻿
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace RayMarchLib
{
    public class DirectBitmap : IDisposable
    {
        public Bitmap Bitmap { get; private set; }
        public byte[] Pixels { get; private set; }
        public bool Disposed { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }

        protected GCHandle BitsHandle { get; private set; }

        public DirectBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Pixels = new byte[height * width * 4];
            BitsHandle = GCHandle.Alloc(Pixels, GCHandleType.Pinned);

            Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppRgb, BitsHandle.AddrOfPinnedObject());
        }

        ~DirectBitmap()
        {
            Dispose(disposing: false);
        }

        public void SetPixel(int x, int y, Color color)
        {
            int index = (x + (y * Width)) * 4;

            int col = color.ToArgb();

            BitConverter.TryWriteBytes(Pixels.AsSpan(index), col);
        }

        public Color GetPixel(int x, int y)
        {
            int index = x + (y * Width);
            int col = Pixels[index];

            return Color.FromArgb(col);
        }

        public void Dispose()
        {
            Dispose(disposing: true);

            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    Bitmap.Dispose();
                }

                BitsHandle.Free();

                Disposed = true;
            }
        }
    }
}