using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using RayMarchLib;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using Windows.Globalization.NumberFormatting;

namespace RayMarchEditor
{
    public sealed partial class EditPage : Page
    {
        private Scene scene;
        private readonly MenuFlyout itemFlyout;
        private readonly DecimalFormatter decimalFormatter;
        private TreeViewNode rightClickedNode;
        // the object currently being edited
        private object editObj;
        // the node and item currently being edited
        private TreeViewNode editNode;
        private TextBlock editBlock;

        public EditPage()
        {
            this.InitializeComponent();

            decimalFormatter = App.Current.Resources["decimalFormatter"] as DecimalFormatter;

            itemFlyout = Resources["itemFlyout"] as MenuFlyout;
        }

        public void LoadScene(Scene scene)
        {
            this.scene = scene;

            FillTree();

            treeView.ExpandAll();

            EditObject(scene);
        }

        private void FillTree()
        {
            treeView.RootNodes.Clear();

            var sceneNode = new TreeViewNode()
            {
                Content = scene
            };

            treeView.RootNodes.Add(sceneNode);

            var objectsNode = new TreeViewNode()
            {
                Content = "Objects"
            };
            
            foreach (RMObject obj in scene.Objects)
            {
                AddObjectToTree(objectsNode, obj);
            }

            sceneNode.Children.Add(objectsNode);
        }

        private void AddObjectToTree(TreeViewNode parentNode, RMObject obj)
        {
            var objectNode = new TreeViewNode()
            {
                Content = obj
            };

            parentNode.Children.Add(objectNode);
        }

        private void EditObject(object obj)
        {
            editObj = obj;

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
                    case true when typeof(float) == pType:
                        EditNumber(info, obj);
                        break;

                    case true when typeof(int) == pType:
                        EditNumber(info, obj, isInt: true);
                        break;

                    case true when typeof(Vector3) == pType:
                        EditVector3(info, obj);
                        break;

                    case true when info.Name == "Name":
                        EditName(obj);
                        break;
                }
            }
        }

        private void EditVector3(PropertyInfo info, object obj)
        {
            var box = new Vector3Box()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Orientation = Orientation.Horizontal
            };
            
            var binding = new Binding()
            {
                Source = obj,
                Path = new PropertyPath(info.Name),
                Mode = BindingMode.TwoWay
            };

            box.SetBinding(Vector3Box.VectorProperty, binding);

            var block = new TextBlock()
            {
                Text = info.Name
            };

            stackPanel.Children.Add(block);
            stackPanel.Children.Add(box);
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

        private void EditName(object obj)
        {
            var box = new TextBox
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Text = obj.GetType().GetProperty("Name").GetValue(obj) as string
            };

            box.TextChanged += NameBox_TextChanged;

            var header = new TextBlock()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Text = "Name"
            };

            stackPanel.Children.Add(header);
            stackPanel.Children.Add(box);
        }

        private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var box = sender as TextBox;
            (editObj as RMObject).Name = box.Text;
            editBlock.Text = editObj.ToString();
        }

        private void TreeViewItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var block = sender as TextBlock;
            var node = block.Tag as TreeViewNode;

            rightClickedNode = node;

            var oType = node.Content.GetType();

            switch (true)
            {
                case true when typeof(Scene) == oType:
                    // Add object
                    itemFlyout.Items[0].Visibility = Visibility.Visible;
                    itemFlyout.Items[1].Visibility = Visibility.Collapsed;
                    break;

                case true when typeof(RMObject).IsAssignableFrom(oType):
                    itemFlyout.Items[0].Visibility = Visibility.Collapsed;
                    // Delete
                    itemFlyout.Items[1].Visibility = Visibility.Visible;
                    break;

                default:
                    foreach (MenuFlyoutItem item in itemFlyout.Items)
                    {
                        item.Visibility = Visibility.Collapsed;
                    }
                    break;
            }
        }

        private void TreeViewItem_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var block = sender as TextBlock;
            var node = block.Tag as TreeViewNode;

            if (node.Content.GetType() != typeof(string))
            {
                editBlock = block;
                editNode = node;

                EditObject(node.Content);
            }
        }

        private async void AddObject_Click(object sender, RoutedEventArgs e)
        {
            var chooseObjectTypePage = new ChooseObjectTypePage();

            var chooseObjectTypeDialog = new ContentDialog()
            {
                XamlRoot = this.XamlRoot,
                Title = "Pick an object type",
                PrimaryButtonText = "OK",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                Content = chooseObjectTypePage
            };

            var res = await chooseObjectTypeDialog.ShowAsync();

            if (res != ContentDialogResult.Primary)
            {
                return;
            }
            
            var obj = Activator.CreateInstance(chooseObjectTypePage.SelectedType) as RMObject;

            scene.Objects.Add(obj);

            TreeViewNode parentNode = treeView.RootNodes[0].Children[0];

            var node = new TreeViewNode()
            {
                Content = obj
            };

            parentNode.Children.Add(node);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            TreeViewNode node = rightClickedNode;

            node.Parent.Children.Remove(node);

            var obj = node.Content as RMObject;
            scene.Objects.Remove(obj);

            if (obj == editObj)
            {
                EditObject(scene);
            }
        }

        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            var box = sender as TextBlock;
            var node = box.Tag as TreeViewNode;

            // The TreeViewNode was deleted from the tree
            if (node == null)
            {
                return;
            }

            object content = node.Content;
            box.Text = content.ToString();

            if (content.GetType() == typeof(string))
            {
                box.ContextFlyout = null;
            }
        }
    }
}
