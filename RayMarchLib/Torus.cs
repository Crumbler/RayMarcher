using System.Globalization;
using System.Numerics;
using System.Xml.Linq;

namespace RayMarchLib
{
    public class Torus : RMObject
    {
        public float InnerRadius { get; set; }
        public float OuterRadius { get; set; }

        public Torus()
        {
            InnerRadius = 0.5f;
            OuterRadius = 0.1f;
        }

        protected override float GetDist(Vector3 v)
        {
            var q = new Vector2(new Vector2(v.X, v.Z).Length() - InnerRadius, v.Y);

            return q.Length() - OuterRadius;
        }

        public override void Serialize(XElement el)
        {
            base.Serialize(el);

            el.Add(new XAttribute(nameof(InnerRadius), InnerRadius.ToString(CultureInfo.InvariantCulture)),
                new XAttribute(nameof(OuterRadius), OuterRadius.ToString(CultureInfo.InvariantCulture)));
        }

        public override void Deserialize(XElement elObj)
        {
            base.Deserialize(elObj);

            InnerRadius = (float)elObj.Attribute(nameof(InnerRadius));
            OuterRadius = (float)elObj.Attribute(nameof(OuterRadius));
        }
    }
}
