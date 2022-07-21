using RayMarchLib;
using Microsoft.UI.Xaml.Media.Imaging;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

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

        public static void ExpandAll(this TreeView treeView)
        {
            var nodes = new Stack<TreeViewNode>();

            if (treeView.RootNodes.Count > 0)
            {
                nodes.Push(treeView.RootNodes[0]);
            }

            while (nodes.TryPop(out TreeViewNode node))
            {
                node.IsExpanded = true;

                foreach (var subnode in node.Children)
                {
                    nodes.Push(subnode);
                }
            }
        }
    }
}
