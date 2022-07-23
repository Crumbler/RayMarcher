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
        private RayMarcher marcher;

        public MainWindow()
        {
            this.InitializeComponent();

            scene = new Scene();

            marcher = new RayMarcher()
            {
                Scene = scene
            };

            rootFrame.Navigate(typeof(EditPage), this);

            Title = "RayMarchEditor";
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Render_Click(object sender, RoutedEventArgs e)
        {
            using var bmp = new DirectBitmap((int)rootFrame.ActualWidth, (int)rootFrame.ActualHeight);

            marcher.Bitmap = bmp;

            marcher.CalculateFrame(threads: 4);

            rootFrame.Navigate(typeof(ImagePage), bmp);

            marcher.Bitmap = null;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            rootFrame.GoBack();
        }
    }
}
