using System.Numerics;

namespace RayMarchLib
{
    public class Subtraction : Intersection
    {
        protected override float GetDist(Vector3 v)
        {
            float maxDist = objects[0].Map(v);

            for (int i = 1; i < objects.Count; ++i)
            {
                float dist = -objects[i].Map(v);
                if (maxDist < dist)
                {
                    maxDist = dist;
                }
            }

            return maxDist;
        }

        protected override HitResult GetHit(Vector3 v)
        {
            float maxDist = objects[0].Map(v);
            RMObject hitObj = objects[0];

            for (int i = 1; i < objects.Count; ++i)
            {
                float dist = -objects[i].Map(v);
                if (maxDist < dist)
                {
                    maxDist = dist;
                    hitObj = objects[i];
                }
            }

            var hit = hitObj.MapHit(v);

            hit.material ??= this.Material;

            return hit;
        }
    }
}
