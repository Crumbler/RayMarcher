using System.Numerics;
using System.Xml.Linq;

namespace RayMarchLib
{
    public class Material
    {
        public Vector3 Color { get; set; }

        public Material()
        {
            Color = Vector3.One;
        }

        public static readonly Material
        Default = new(),
        Background = new()
        {
            Color = Vector3.Zero
        };

        public static Material ParseMaterial(XElement el)
        {
            var m = new Material();

            XAttribute attrColor = el.Attribute(nameof(Color));
            if (attrColor is not null)
            {
                m.Color = Utils.ToVec3(attrColor.Value);
            }

            return m;
        }
    }
}
