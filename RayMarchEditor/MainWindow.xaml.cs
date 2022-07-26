﻿using Microsoft.UI.Xaml;
using System;
using RayMarchLib;
using System.Diagnostics;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;
using Microsoft.UI.Xaml.Controls;

namespace RayMarchEditor
{
    public sealed partial class MainWindow : Window
    {
        public Scene scene;
        private RayMarcher marcher;
        private FileSavePicker saveScenePicker, saveImagePicker;
        private FileOpenPicker openScenePicker;

        public MainWindow()
        {
            this.InitializeComponent();

            Title = "RayMarchEditor";

            marcher = new RayMarcher();

            InitPickers();

            rootFrame.Navigate(typeof(EditPage));

            LoadScene(new Scene());
        }

        private void InitPickers()
        {
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

            saveImagePicker = new FileSavePicker()
            {
                SuggestedStartLocation = PickerLocationId.ComputerFolder,
                SuggestedFileName = "scene"
            };

            saveImagePicker.FileTypeChoices.Add("Image", new string[] { ".png" });

            InitializeWithWindow.Initialize(saveImagePicker, hwnd);
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

        private async void MenuSaveImage_Click(object sender, RoutedEventArgs e)
        {
            var saveImagePage = new SaveImagePage();

            var saveImageDialog = new ContentDialog()
            {
                XamlRoot = this.Content.XamlRoot,
                Title = "Edit image settings",
                PrimaryButtonText = "OK",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                Content = saveImagePage
            };

            var res = await saveImageDialog.ShowAsync();

            if (res != ContentDialogResult.Primary)
            {
                return;
            }

            StorageFile file = await saveImagePicker.PickSaveFileAsync();
            if (file == null)
            {
                return;
            }

            using var bmp = new DirectBitmap(saveImagePage.ImageWidth, saveImagePage.ImageHeight);

            marcher.Bitmap = bmp;
            marcher.CalculateFrame(4);

            bmp.Bitmap.Save(file.Path);

            marcher.Bitmap = null;
        }
    }
}
