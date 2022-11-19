using System.Globalization;
using System.Numerics;
using System.Xml.Linq;

namespace RayMarchLib
{
    public class Capsule : RMObject
    {
        public float Length { get; set; }
        public float Radius { get; set; }

        public Capsule()
        {
            Length = 1.0f;
            Radius = 0.1f;
        }

        protected override float GetDist(Vector3 v)
        {
            v.Y -= Utils.Clamp(v.Y, -Length / 2.0f, Length / 2.0f);

            return v.Length() - Radius;
        }

        public override void Serialize(XElement el)
        {
            base.Serialize(el);

            el.Add(new XAttribute(nameof(Length), Length.ToString(CultureInfo.InvariantCulture)),
                new XAttribute(nameof(Radius), Radius.ToString(CultureInfo.InvariantCulture)));
        }

        public override void Deserialize(XElement elObj)
        {
            base.Deserialize(elObj);

            Length = (float)elObj.Attribute(nameof(Length));
            Radius = (float)elObj.Attribute(nameof(Radius));
        }
    }
}
