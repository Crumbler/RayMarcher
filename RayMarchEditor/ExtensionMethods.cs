using RayMarchLib;
using Microsoft.UI.Xaml.Media.Imaging;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;

namespace RayMarchEditor
{
    public static class ExtensionMethods
    {
        public static WriteableBitmap ToWriteableBitmap(this DirectBitmap bmp)
        {
            var wrBmp = new WriteableBitmap(bmp.Width, bmp.Height);

            using Stream pixelStream = wrBmp.PixelBuffer.AsStream();

            pixelStream.Write(bmp.Pixels);

            return wrBmp;
        }
    }
}
