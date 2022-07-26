using Microsoft.UI.Xaml.Controls;

namespace RayMarchEditor
{

    public sealed partial class SaveImagePage : Page
    {
        public int ImageWidth { get; private set; }
        public int ImageHeight { get; private set; }

        public SaveImagePage()
        {
            this.InitializeComponent();
        }
    }
}
