
using System.Numerics;

namespace RayMarchLib
{
    public class Sphere : RMObject
    {
        public float Radius { get; set; }

        public Sphere(float radius)
        {
            Radius = radius;
        }

        protected override float GetDist(Vector3 v)
        {
            return v.Length() - Radius;
        }
    }
}
