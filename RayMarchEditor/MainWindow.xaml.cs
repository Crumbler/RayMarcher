using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using RayMarchLib;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RayMarchEditor
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public Scene scene;

        public MainWindow()
        {
            this.InitializeComponent();

            scene = new Scene();

            rootFrame.Navigate(typeof(EditPage), this);

            Title = "RayMarchEditor";
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Render_Click(object sender, RoutedEventArgs e)
        {
            using var bmp = new DirectBitmap(1280, 720);
            for (int i = 0; i < 99; ++i)
            {
                bmp.SetPixel(i, i, System.Drawing.Color.Red);
            }

            rootFrame.Navigate(typeof(ImagePage), bmp);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            rootFrame.GoBack();
        }
    }
}
