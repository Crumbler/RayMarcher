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
            return q.MaxZero().Length() + q.Max().MinZero();
        }

        public override void Deserialize(Dictionary<string, Material> materials, XElement elObj)
        {
            base.Deserialize(materials, elObj);

            XAttribute attrSize = elObj.Attribute(nameof(Size));
            if (attrSize is not null)
            {
                Size = Utils.ToVec3(attrSize.Value);

                if (Size.X < 0f || Size.Y < 0f || Size.Z < 0f)
                {
                    throw new SceneDeserializationException("Box Size cannot be negative");
                }
            }
        }
    }
}
