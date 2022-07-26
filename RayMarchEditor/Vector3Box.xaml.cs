using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Numerics;

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

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public static readonly DependencyProperty VectorProperty =
            DependencyProperty.Register(
                nameof(Vector),
                typeof(Vector3),
                typeof(Vector3Box),
                new PropertyMetadata(default(Vector3)));

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                nameof(Orientation),
                typeof(Orientation),
                typeof(Vector3Box),
                new PropertyMetadata(Orientation.Horizontal));
    }
}
