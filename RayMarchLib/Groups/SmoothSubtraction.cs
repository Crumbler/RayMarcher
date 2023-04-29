using System.Numerics;

namespace RayMarchLib
{
    public class SmoothSubtraction : SmoothUnion
    {
        protected override float GetDist(Vector3 v)
        {
            float maxDist = objects[0].Map(v);

            for (int i = 1; i < objects.Count; ++i)
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
            HitResult initialHit = objects[0].MapHit(v);
            float maxDist = initialHit.distance;
            Material currM = initialHit.material ?? RayMarchLib.Material.Default;

            for (int i = 1; i < objects.Count; ++i)
            {
                HitResult res = objects[i].MapHit(v);
                Material objMat = res.material ?? RayMarchLib.Material.Default;

                (float dist, Material newMat) = Utils.SmoothIntersection(currM, objMat, maxDist, -res.distance, BlendRadius);
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
