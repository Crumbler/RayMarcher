using System;
using System.Collections.Generic;
using System.Numerics;
using System.Xml.Linq;

namespace RayMarchLib
{
    public class Capsule : RMObject
    {
        public float Length { get; set; } = 1.0f;
        public float Radius { get; set; } = 0.1f;

        protected override float GetDist(Vector3 v)
        {
            v = Vector3.Abs(v);
            v.Y = MathF.Max(0f, v.Y - Length / 2f);

            return v.Length() - Radius;
        }

        public override void Deserialize(Dictionary<string, Material> materials, XElement elObj)
        {
            base.Deserialize(materials, elObj);

            XAttribute attrLength = elObj.Attribute(nameof(Length));
            if (attrLength is not null)
            {
                Length = Utils.ParseFloat(attrLength.Value);
            }

            XAttribute attrRadius = elObj.Attribute(nameof(Radius));
            if (attrRadius is not null)
            {
                Radius = Utils.ParseFloat(attrRadius.Value);
            }
        }
    }
}
