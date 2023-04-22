using System.Collections.Generic;
using System.Numerics;
using System.Xml.Linq;

namespace RayMarchLib
{
    public class Sphere : RMObject
    {
        public float Radius { get; set; } = 1.0f;

        protected override float GetDist(Vector3 v)
        {
            return v.Length() - Radius;
        }

        public override void Deserialize(Dictionary<string, Material> materials, XElement elObj)
        {
            base.Deserialize(materials, elObj);

            XAttribute attrRadius = elObj.Attribute(nameof(Radius));
            if (attrRadius is not null)
            {
                Radius = Utils.ParseFloat(attrRadius.Value);

                if (Radius <= 0f)
                {
                    throw new SceneDeserializationException("Radius must be positive");
                }
            }
        }
    }
}
