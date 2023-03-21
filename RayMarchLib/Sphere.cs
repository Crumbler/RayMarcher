using System.Globalization;
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

        public override void Serialize(XElement el)
        {
            base.Serialize(el);

            el.Add(new XAttribute(nameof(Radius), Radius.ToString(CultureInfo.InvariantCulture)));
        }

        public override void Deserialize(XElement elObj)
        {
            base.Deserialize(elObj);

            XAttribute attrRadius = elObj.Attribute(nameof(Radius));
            if (attrRadius is not null)
            {
                Radius = float.Parse(attrRadius.Value, CultureInfo.InvariantCulture);
            }
        }
    }
}
