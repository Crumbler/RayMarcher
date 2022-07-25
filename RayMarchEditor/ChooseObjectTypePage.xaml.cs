using Microsoft.UI.Xaml.Controls;
using System;
using RayMarchLib;

namespace RayMarchEditor
{
    public sealed partial class ChooseObjectTypePage : Page
    {
        public Type SelectedType { get => Scene.objectTypes[SelIndex]; }
        private int SelIndex { get; set; }
        public ChooseObjectTypePage()
        {
            this.InitializeComponent();

            foreach (Type t in Scene.objectTypes)
            {
                comboBox.Items.Add(t.Name);
            }
        }
    }
}
