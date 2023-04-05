using System;
using System.Collections.Generic;
using System.Numerics;
using System.Xml.Linq;

namespace RayMarchLib
{
    public class Cyclinder : RMObject
    {
        public float Length { get; set; } = 1f;
        public float Radius { get; set; } = 0.1f;

        protected override float GetDist(Vector3 v)
        {
            Vector2 d = Vector2.Abs(new Vector2(v.XZ().Length(), v.Y)) -
                new Vector2(Radius, Length / 2f);

            return MathF.Max(d.X, d.Y).MinZero() + Vector2.Max(d, Vector2.Zero).Length();
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
