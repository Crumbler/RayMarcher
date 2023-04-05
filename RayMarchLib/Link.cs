using System;
using System.Collections.Generic;
using System.Numerics;
using System.Xml.Linq;

namespace RayMarchLib
{
    public class Link : RMObject
    {
        public float InnerRadius { get; set; } = 0.2f;
        public float OuterRadius { get; set; } = 0.07f;
        public float Length { get; set; } = 0.2f;

        protected override float GetDist(Vector3 v)
        {
            var q = new Vector3(v.X, (MathF.Abs(v.Y) - Length).MaxZero(), v.Z);
            return new Vector2(q.XY().Length() - InnerRadius, q.Z).Length() - OuterRadius;
        }

        public override void Deserialize(Dictionary<string, Material> materials, XElement elObj)
        {
            base.Deserialize(materials, elObj);

            XAttribute attrLength = elObj.Attribute(nameof(Length));
            if (attrLength is not null)
            {
                Length = Utils.ParseFloat(attrLength.Value);
            }

            XAttribute attrInnerRadius = elObj.Attribute(nameof(InnerRadius));
            if (attrInnerRadius is not null)
            {
                InnerRadius = Utils.ParseFloat(attrInnerRadius.Value);
            }

            XAttribute attrOuterRadius = elObj.Attribute(nameof(OuterRadius));
            if (attrOuterRadius is not null)
            {
                OuterRadius = Utils.ParseFloat(attrOuterRadius.Value);
            }
        }
    }
}
