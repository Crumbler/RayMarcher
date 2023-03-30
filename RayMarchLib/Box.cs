using System;
using System.Collections.Generic;
using System.Numerics;
using System.Xml.Linq;

namespace RayMarchLib
{
    public class Box : RMObject
    {
        public Vector3 Size { get; set; } = new Vector3(0.5f);

        protected override float GetDist(Vector3 v)
        {
            var q = Vector3.Abs(v) - Size;
            return Vector3.Max(q, Vector3.Zero).Length() + 
                MathF.Min(MathF.Max(q.X, Math.Max(q.Y, q.Z)), 0.0f);
        }

        public override void Deserialize(Dictionary<string, Material> materials, XElement elObj)
        {
            base.Deserialize(materials, elObj);

            XAttribute attrSize = elObj.Attribute(nameof(Size));
            if (attrSize is not null)
            {
                Size = Utils.ToVec3(attrSize.Value);
            }
        }
    }
}
