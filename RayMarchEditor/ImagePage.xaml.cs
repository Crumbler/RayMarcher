using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using RayMarchLib;

namespace RayMarchEditor
{
    public sealed partial class ImagePage : Page
    {
        public ImagePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter != null)
            {
                imgMain.Source = (e.Parameter as DirectBitmap).ToWriteableBitmap();
            }
        }
    }
}
