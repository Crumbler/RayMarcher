using System.Drawing;

namespace RayMarchLib
{
    public class Material
    {
        public Color Color { get; set; }

        public string Name { get; set; }

        public Material()
        {
            Color = Color.White;
        }

        public static readonly Material
        Default = new()
        {
            Name = "Default"
        },
        Background = new()
        {
            Color = Color.Black
        };

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return GetType().Name;
            }

            return Name;
        }
    }
}
