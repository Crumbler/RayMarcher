using System;
using System.Collections.Generic;
using System.Numerics;
using System.Xml.Linq;

namespace RayMarchLib
{
    public class BoxFrame : RMObject
    {
        public Vector3 Size { get; set; } = new Vector3(0.4f);
        public float Thickness { get; set; } = 0.05f;

        protected override float GetDist(Vector3 v)
        {
            v = Vector3.Abs(v) - Size;
            var q = Vector3.Abs(v + new Vector3(Thickness)) - new Vector3(Thickness);

            return Utils.Min(
                Vector3.Max(new Vector3(v.X, q.Y, q.Z), Vector3.Zero).Length() +
                    MathF.Min(0f, Utils.Max(v.X, q.Y, q.Z)),
                Vector3.Max(new Vector3(q.X, v.Y, q.Z), Vector3.Zero).Length() +
                    MathF.Min(0f, Utils.Max(q.X, v.Y, q.Z)),
                Vector3.Max(new Vector3(q.X, q.Y, v.Z), Vector3.Zero).Length() +
                    MathF.Min(0f, Utils.Max(q.X, q.Y, v.Z)));
        }

        public override void Deserialize(Dictionary<string, Material> materials, XElement elObj)
        {
            base.Deserialize(materials, elObj);

            XAttribute attrSize = elObj.Attribute(nameof(Size));
            if (attrSize is not null)
            {
                Size = Utils.ToVec3(attrSize.Value);
            }

            XAttribute attrThickness = elObj.Attribute(nameof(Thickness));
            if (attrThickness is not null)
            {
                Thickness = Utils.ParseFloat(attrThickness.Value);
            }
        }
    }
}
