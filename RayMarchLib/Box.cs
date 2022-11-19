using System;
using System.Numerics;
using System.Xml.Linq;

namespace RayMarchLib
{
    public class Box : RMObject
    {
        public Vector3 Size { get; set; }

        public Box()
        {
            Size = new Vector3(0.5f);
        }

        protected override float GetDist(Vector3 v)
        {
            var q = Vector3.Abs(v) - Size;
            return Vector3.Max(q, Vector3.Zero).Length() + 
                MathF.Min(MathF.Max(q.X, Math.Max(q.Y, q.Z)), 0.0f);
        }

        public override void Serialize(XElement el)
        {
            base.Serialize(el);

            el.Add(new XAttribute(nameof(Size), Utils.ToString(Size)));
        }

        public override void Deserialize(XElement elObj)
        {
            base.Deserialize(elObj);

            Size = Utils.ToVec3(elObj.Attribute(nameof(Size)).Value);
        }
    }
}
