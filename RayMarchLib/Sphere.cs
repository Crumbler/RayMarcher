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

        public override void Deserialize(XElement elObj)
        {
            base.Deserialize(elObj);

            XAttribute attrRadius = elObj.Attribute(nameof(Radius));
            if (attrRadius is not null)
            {
                Radius = Utils.ParseFloat(attrRadius.Value);
            }
        }
    }
}
