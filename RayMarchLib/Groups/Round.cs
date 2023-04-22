using System.Collections.Generic;
using System.Numerics;
using System.Xml.Linq;

namespace RayMarchLib
{
    public class Round : RMObject, IRMGroup
    {
        private RMObject obj;
        public float Radius { get; set; } = 0.05f;

        public void AddObject(RMObject obj)
        {
            if (this.obj is not null)
            {
                throw new SceneDeserializationException("Only one object can be rounded.");
            }

            this.obj = obj;
        }

        public override void PreCalculate()
        {
            base.PreCalculate();

            obj.PreCalculate();
        }

        protected override HitResult GetHit(Vector3 v)
        {
            var hit = obj.MapHit(v);

            hit.material ??= this.Material;

            return hit;
        }

        protected override float GetDist(Vector3 v)
        {
            return obj.Map(v) - Radius;
        }

        public override void Deserialize(Dictionary<string, Material> materials, XElement elObj)
        {
            base.Deserialize(materials, elObj);

            XAttribute attrRadius = elObj.Attribute(nameof(Radius));
            if (attrRadius is not null)
            {
                Radius = Utils.ParseFloat(attrRadius.Value);
            }
        }
    }
}
