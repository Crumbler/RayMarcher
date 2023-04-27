using System;
using System.Collections.Generic;
using System.Numerics;
using System.Xml.Linq;

namespace RayMarchLib
{
    public class Blend : RMObject, IRMGroup
    {
        public float BlendFactor { get; set; } = 0.5f;

        RMObject objA, objB;

        protected override float GetDist(Vector3 v)
        {
            float distA = objA.Map(v),
                distB = objB.Map(v);

            return Utils.Mix(distA, distB, BlendFactor);
        }

        protected override HitResult GetHit(Vector3 v)
        {
            HitResult resA = objA.MapHit(v),
                resB = objB.MapHit(v);

            Material matA = resA.material ?? RayMarchLib.Material.Default,
                matB = resB.material ?? RayMarchLib.Material.Default;

            return new HitResult()
            {
                distance = Utils.Mix(resA.distance, resB.distance, BlendFactor),
                material = Utils.Mix(matA, matB, BlendFactor)
            };
        }

        public void AddObject(RMObject obj)
        {
            if (objA == null)
            {
                objA = obj;
            }
            else if (objB == null)
            {
                objB = obj;
            }
            else
            {
                throw new SceneDeserializationException("Blend must contain 2 objects");
            }
        }

        public override void Deserialize(Dictionary<string, Material> materials, XElement elObj)
        {
            base.Deserialize(materials, elObj);

            XAttribute attrBlendFactor = elObj.Attribute(nameof(BlendFactor));
            if (attrBlendFactor is not null)
            {
                BlendFactor = Utils.ParseFloat(attrBlendFactor.Value);

                if (BlendFactor != Utils.Clamp(BlendFactor, 0f, 1f))
                {
                    throw new SceneDeserializationException("BlendFactor must be in the range [0; 1]");
                }
            }
        }

        public override void PreCalculate()
        {
            base.PreCalculate();

            objA.PreCalculate();
            objB.PreCalculate();
        }
    }
}
