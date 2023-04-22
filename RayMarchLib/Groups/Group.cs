using System.Collections.Generic;
using System.Numerics;

namespace RayMarchLib
{
    public class Group : RMObject, IRMGroup
    {
        protected readonly List<RMObject> objects = new();
        protected override float GetDist(Vector3 v)
        {
            float minDist = float.PositiveInfinity;

            for (int i = 0; i < objects.Count; ++i)
            {
                float dist = objects[i].Map(v);
                if (minDist > dist)
                {
                    minDist = dist;
                }
            }

            return minDist;
        }

        protected override HitResult GetHit(Vector3 v)
        {
            float minDist = float.PositiveInfinity;
            RMObject hitObj = null;

            for (int i = 0; i < objects.Count; ++i)
            {
                float dist = objects[i].Map(v);
                if (minDist > dist)
                {
                    minDist = dist;
                    hitObj = objects[i];
                }
            }

            var hit = hitObj.MapHit(v);

            hit.material ??= this.Material;

            return hit;
        }

        public void AddObject(RMObject obj)
        {
            objects.Add(obj);
        }

        public override void PreCalculate()
        {
            base.PreCalculate();

            foreach (RMObject obj in objects)
            {
                obj.PreCalculate();
            }
        }
    }
}
