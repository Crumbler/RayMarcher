using System.Collections.Generic;
using System.Numerics;
using System.Xml.Linq;

namespace RayMarchLib
{
    public class SmoothUnion : Group
    {
        public float BlendRadius { get; set; } = 0.1f;

        protected override float GetDist(Vector3 v)
        {
            float minDist = float.MaxValue;

            for (int i = 0; i < objects.Count; ++i)
            {
                float dist = Utils.SmoothMin(objects[i].Map(v), minDist, BlendRadius);
                if (minDist > dist)
                {
                    minDist = dist;
                }
            }

            return minDist;
        }

        protected override HitResult GetHit(Vector3 v)
        {
            float minDist = float.MaxValue;
            Material currM = RayMarchLib.Material.Default;

            for (int i = 0; i < objects.Count; ++i)
            {
                Material objMat = objects[i].Material ?? RayMarchLib.Material.Default;

                (float dist, Material newMat) = Utils.SmoothMin(currM, objMat, minDist, objects[i].Map(v), BlendRadius);
                if (minDist > dist)
                {
                    minDist = dist;
                    currM = newMat;
                }
            }

            return new HitResult()
            {
                distance = minDist,
                material = currM
            };
        }

        public override void Deserialize(Dictionary<string, Material> materials, XElement elObj)
        {
            base.Deserialize(materials, elObj);

            XAttribute attrBlendRadius = elObj.Attribute(nameof(BlendRadius));
            if (attrBlendRadius is not null)
            {
                BlendRadius = Utils.ParseFloat(attrBlendRadius.Value);

                if (BlendRadius <= 0f)
                {
                    throw new SceneDeserializationException("BlendRadius must be positive");
                }
            }
        }
    }
}
