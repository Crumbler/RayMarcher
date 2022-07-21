using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Diagnostics;
using System.Reflection;
using Windows.Globalization.NumberFormatting;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RayMarchEditor
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditPage : Page
    {
        private MainWindow window;
        private Frame rootFrame;
        private bool initialized;
        private MenuFlyout itemFlyout;
        private IncrementNumberRounder rounder;
        private DecimalFormatter decimalFormatter;

        public EditPage()
        {
            this.InitializeComponent();

            initialized = false;

            itemFlyout = Resources["itemFlyout"] as MenuFlyout;

            rootFrame = this.Parent as Frame;

            NavigationCacheMode = NavigationCacheMode.Required;

            rounder = new IncrementNumberRounder()
            {
                RoundingAlgorithm = RoundingAlgorithm.RoundHalfUp
            };

            decimalFormatter = new DecimalFormatter()
            {
                FractionDigits = 0,
                
                NumberRounder = rounder
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (initialized)
            {
                return;
            }

            initialized = true;

            window = e.Parameter as MainWindow;

            FillTree();

            treeView.ExpandAll();

            EditObject(window.scene);
        }

        private void FillTree()
        {
            var sceneNode = new TreeViewNode()
            {
                Content = window.scene
            };

            treeView.RootNodes.Add(sceneNode);

            var node = new TreeViewNode()
            {
                Content = "Objects"
            };

            sceneNode.Children.Add(node);
        }

        private void EditObject(object obj)
        {
            stackPanel.Children.Clear();

            var objectDesc = new TextBlock()
            {
                Text = $"Editing {obj}"
            };

            stackPanel.Children.Add(objectDesc);

            var properties = obj.GetType().GetProperties();
            
            foreach (PropertyInfo info in properties)
            {
                Type pType = info.PropertyType;

                switch (true)
                {
                    case true when typeof(float).IsAssignableFrom(pType):
                        EditNumber(info, obj);
                        break;
                    case true when typeof(int).IsAssignableFrom(pType):
                        EditNumber(info, obj, isInt: true);
                        break;
                }
            }
        }

        private void EditNumber(PropertyInfo info, object obj, bool isInt = false)
        {
            var box = new NumberBox
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Header = info.Name
            };
            
            var binding = new Binding()
            {
                Source = obj,
                Path = new PropertyPath(info.Name),
                Mode = BindingMode.TwoWay
            };

            box.SetBinding(NumberBox.ValueProperty, binding);

            if (isInt)
            {
                box.NumberFormatter = decimalFormatter;
            }

            stackPanel.Children.Add(box);
        }

        private void TreeViewItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var item = sender as TreeViewItem;

            var node = item.Content as TreeViewNode;

            Debug.WriteLine(node.Content.GetType().ToString());
        }
    }
}
