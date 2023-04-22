using System.Numerics;

namespace RayMarchLib
{
    public class SmoothSubtraction : SmoothUnion
    {
        protected override float GetDist(Vector3 v)
        {
            float maxDist = objects[^1].Map(v);

            for (int i = objects.Count - 2; i >= 0; --i)
            {
                float dist = Utils.SmoothIntersection(maxDist, -objects[i].Map(v), BlendRadius);
                if (maxDist < dist)
                {
                    maxDist = dist;
                }
            }

            return maxDist;
        }

        protected override HitResult GetHit(Vector3 v)
        {
            float maxDist = objects[^1].Map(v);
            Material currM = objects[^1].Material ?? RayMarchLib.Material.Default;

            for (int i = objects.Count - 2; i >= 0; --i)
            {
                Material objMat = objects[i].Material ?? RayMarchLib.Material.Default;

                (float dist, Material newMat) = Utils.SmoothIntersection(currM, objMat, maxDist, -objects[i].Map(v), BlendRadius);
                if (maxDist < dist)
                {
                    maxDist = dist;
                    currM = newMat;
                }
            }

            return new HitResult()
            {
                distance = maxDist,
                material = currM
            };
        }

        public override void PreCalculate()
        {
            base.PreCalculate();

            if (objects.Count <= 1)
            {
                throw new SceneDeserializationException($"{GetType().Name} must contain at least 2 objects.");
            }
        }
    }
}
