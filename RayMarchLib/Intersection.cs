
using System.Collections.Generic;
using System.Numerics;

namespace RayMarchLib
{
    public class Intersection : RMObject, IRMGroup
    {
        private readonly List<RMObject> objects = new();
        protected override float GetDist(Vector3 v)
        {
            float maxDist = objects[^1].Map(v);

            for (int i = objects.Count - 2; i >= 0; --i)
            {
                float dist = objects[i].Map(v);
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
            RMObject hitObj = objects[^1];

            for (int i = objects.Count - 2; i >= 0; --i)
            {
                float dist = objects[i].Map(v);
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

            if (objects.Count <= 1)
            {
                throw new SceneDeserializationException("Intersection must contain at least 2 objects.");
            }
        }
    }
}
