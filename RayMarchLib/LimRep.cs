using System.Collections.Generic;
using System.Numerics;
using System.Xml.Linq;

namespace RayMarchLib
{
    public class LimRep : InfRep
    {
        public Vector3 LowerBound { get; set; } = -Vector3.One;
        public Vector3 UpperBound { get; set; } = Vector3.One;

        protected override Vector3 Repeat(Vector3 v)
        {
            return v - CellSize * Vector3.Clamp(Utils.Round(v / CellSize), LowerBound, UpperBound);
        }

        public override void Deserialize(Dictionary<string, Material> materials, XElement elObj)
        {
            base.Deserialize(materials, elObj);

            XAttribute attrLowerBound = elObj.Attribute(nameof(LowerBound));
            if (attrLowerBound is not null)
            {
                LowerBound = Utils.ToVec3(attrLowerBound.Value);

                if (Utils.HasFraction(LowerBound))
                {
                    throw new SceneDeserializationException("No component of LowerBound may have a fractional part");
                }
            }

            XAttribute attrUpperBound = elObj.Attribute(nameof(UpperBound));
            if (attrUpperBound is not null)
            {
                UpperBound = Utils.ToVec3(attrUpperBound.Value);

                if (Utils.HasFraction(UpperBound))
                {
                    throw new SceneDeserializationException("No component of UpperBound may have a fractional part");
                }
            }

            if (LowerBound.X > UpperBound.X || LowerBound.Y > UpperBound.Y ||
                LowerBound.Z > UpperBound.Z)
            {
                throw new SceneDeserializationException("LowerBound must be higher than UpperBound");
            }
        }
    }
}
