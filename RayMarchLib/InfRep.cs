using System.Collections.Generic;
using System.Numerics;
using System.Xml.Linq;

namespace RayMarchLib
{
    public class InfRep : Group
    {
        public Vector3 CellSize { get; set; } = Vector3.One;

        protected virtual Vector3 Repeat(Vector3 v)
        {
            return Utils.Mod(v + CellSize / 2f, CellSize);
        }

        protected override float GetDist(Vector3 v)
        {
            v = Repeat(v);

            return base.GetDist(v);
        }

        protected override HitResult GetHit(Vector3 v)
        {
            v = Repeat(v);

            return base.GetHit(v);
        }

        public override void Deserialize(Dictionary<string, Material> materials, XElement elObj)
        {
            base.Deserialize(materials, elObj);

            XAttribute attrCellSize = elObj.Attribute(nameof(CellSize));
            if (attrCellSize is not null)
            {
                CellSize = Utils.ToVec3(attrCellSize.Value);

                if (CellSize.X < 0f || CellSize.Y < 0f || CellSize.Z < 0f)
                {
                    throw new SceneDeserializationException("CellSize must be non-negative");
                }
            }
        }
    }
}
