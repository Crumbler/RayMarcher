
using System.Numerics;

namespace RayMarchLib
{
    public class Sphere : RMObject
    {
        public float Radius { get; set; }

        public Sphere()
        {
            Radius = 1.0f;
        }

        protected override float GetDist(Vector3 v)
        {
            return v.Length() - Radius;
        }
    }
}
