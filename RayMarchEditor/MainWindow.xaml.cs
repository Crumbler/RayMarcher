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
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;

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
        private FileSavePicker saveScenePicker;
        private FileOpenPicker openScenePicker;

        public MainWindow()
        {
            this.InitializeComponent();

            Title = "RayMarchEditor";

            marcher = new RayMarcher();

            IntPtr hwnd = WindowNative.GetWindowHandle(this);

            saveScenePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.ComputerFolder,
                SuggestedFileName = "scene"
            };

            saveScenePicker.FileTypeChoices.Add("XML", new string[] { ".xml" });

            InitializeWithWindow.Initialize(saveScenePicker, hwnd);

            openScenePicker = new FileOpenPicker()
            {
                SuggestedStartLocation = PickerLocationId.ComputerFolder,
                ViewMode = PickerViewMode.List
            };

            openScenePicker.FileTypeFilter.Add(".xml");

            InitializeWithWindow.Initialize(openScenePicker, hwnd);

            rootFrame.Navigate(typeof(EditPage));

            LoadScene(new Scene());
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

        private void MenuNew_Click(object sender, RoutedEventArgs e)
        {
            LoadScene(new Scene());
        }

        private void LoadScene(Scene scene)
        {
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }

            var page = rootFrame.Content as EditPage;

            this.scene = scene;
            marcher.Scene = scene;

            page.LoadScene(scene);
        }

        private async void MenuSaveScene_Click(object sender, RoutedEventArgs e)
        {
            StorageFile file = await saveScenePicker.PickSaveFileAsync();
            if (file == null)
            {
                return;
            }

            scene.SerializeToFile(file.Path);
        }

        private async void MenuOpenScene_Click(object sender, RoutedEventArgs e)
        {
            StorageFile file = await openScenePicker.PickSingleFileAsync();
            if (file == null)
            {
                return;
            }

            var scene = Scene.LoadFromFile(file.Path);

            LoadScene(scene);
        }
    }
}
