using System.Collections.Generic;
using System.Numerics;
using System.Xml.Linq;

namespace RayMarchLib
{
    public class Torus : RMObject
    {
        public float InnerRadius { get; set; } = 0.5f;
        public float OuterRadius { get; set; } = 0.1f;

        protected override float GetDist(Vector3 v)
        {
            var q = new Vector2(v.XZ().Length() - InnerRadius, v.Y);

            return q.Length() - OuterRadius;
        }

        public override void Deserialize(Dictionary<string, Material> materials, XElement elObj)
        {
            base.Deserialize(materials, elObj);

            XAttribute attrInnerRadius = elObj.Attribute(nameof(InnerRadius));
            if (attrInnerRadius is not null)
            {
                InnerRadius = Utils.ParseFloat(attrInnerRadius.Value);

                if (InnerRadius <= 0f)
                {
                    throw new SceneDeserializationException("InnerRadius must be positive");
                }
            }

            XAttribute attrOuterRadius = elObj.Attribute(nameof(OuterRadius));
            if (attrOuterRadius is not null)
            {
                OuterRadius = Utils.ParseFloat(attrOuterRadius.Value);

                if (OuterRadius <= 0f)
                {
                    throw new SceneDeserializationException("OuterRadius must be positive");
                }
            }
        }
    }
}
