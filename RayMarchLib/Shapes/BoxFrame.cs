using System.Collections.Generic;
using System.Numerics;
using System.Xml.Linq;

namespace RayMarchLib
{
    public class BoxFrame : Box
    {
        public float Thickness { get; set; } = 0.05f;

        protected override float GetDist(Vector3 v)
        {
            v = Vector3.Abs(v) - Size;
            var q = Vector3.Abs(v + new Vector3(Thickness)) - new Vector3(Thickness);

            Vector3 a = new(q.YZ(), v.X),
                b = new(q.XZ(), v.Y),
                c = new(q.XY(), v.Z);

            return Utils.Min(
                a.MaxZero().Length() + a.Max().MinZero(),
                b.MaxZero().Length() + b.Max().MinZero(),
                c.MaxZero().Length() + c.Max().MinZero());
        }

        public override void Deserialize(Dictionary<string, Material> materials, XElement elObj)
        {
            base.Deserialize(materials, elObj);

            XAttribute attrThickness = elObj.Attribute(nameof(Thickness));
            if (attrThickness is not null)
            {
                Thickness = Utils.ParseFloat(attrThickness.Value);

                if (Thickness <= 0f)
                {
                    throw new SceneDeserializationException("Thickness must be positive");
                }
            }
        }
    }
}
