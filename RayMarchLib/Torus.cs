using System.Globalization;
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
            var q = new Vector2(new Vector2(v.X, v.Z).Length() - InnerRadius, v.Y);

            return q.Length() - OuterRadius;
        }

        public override void Deserialize(XElement elObj)
        {
            base.Deserialize(elObj);

            XAttribute attrInnerRadius = elObj.Attribute(nameof(InnerRadius));
            if (attrInnerRadius is not null)
            {
                InnerRadius = float.Parse(attrInnerRadius.Value, CultureInfo.InvariantCulture);
            }

            XAttribute attrOuterRadius = elObj.Attribute(nameof(OuterRadius));
            if (attrOuterRadius is not null)
            {
                OuterRadius = float.Parse(attrOuterRadius.Value, CultureInfo.InvariantCulture);
            }
        }
    }
}
