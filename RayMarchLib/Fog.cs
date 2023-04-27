using System.Numerics;
using System.Xml.Linq;

namespace RayMarchLib
{
    public class Fog
    {
        public Vector3 Color { get; set; } = Vector3.One;
        public float FalloffFactor { get; set; } = 1f;

        public static Fog ParseFog(XElement el)
        {
            var f = new Fog();

            XAttribute attrColor = el.Attribute(nameof(Color));
            if (attrColor is not null)
            {
                f.Color = Utils.ToVec3(attrColor.Value);

                if (Vector3.Clamp(f.Color, Vector3.Zero, Vector3.One) != f.Color)
                {
                    throw new SceneDeserializationException($"Color must be between {Vector3.Zero} and {Vector3.One}");
                }
            }

            XAttribute attrFalloffFactor = el.Attribute(nameof(FalloffFactor));
            if (attrFalloffFactor is not null)
            {
                f.FalloffFactor = Utils.ParseFloat(attrFalloffFactor.Value);

                if (f.FalloffFactor < 0f)
                {
                    throw new SceneDeserializationException("FalloffFactor cannot be negative");
                }
            }

            return f;
        }
    }
}
