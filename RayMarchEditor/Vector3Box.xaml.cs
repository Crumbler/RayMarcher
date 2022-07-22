using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RayMarchEditor
{
    public sealed partial class Vector3Box : UserControl
    {
        public Vector3Box()
        {
            this.InitializeComponent();
        }

        public Vector3 Vector
        {
            get => (Vector3)GetValue(VectorProperty);
            set => SetValue(VectorProperty, value);
        }

        public float X 
        {
            get => Vector.X;
            set => Vector = new Vector3(value, Vector.Y, Vector.Z);
        }

        public float Y
        {
            get => Vector.Y;
            set => Vector = new Vector3(Vector.X, value, Vector.Z);
        }

        public float Z
        {
            get => Vector.Z;
            set => Vector = new Vector3(Vector.X, Vector.Y, value);
        }

        public static readonly DependencyProperty VectorProperty =
            DependencyProperty.Register(
                nameof(Vector),
                typeof(Vector3),
                typeof(Vector3Box),
                new PropertyMetadata(default(Vector3)));
    }
}
