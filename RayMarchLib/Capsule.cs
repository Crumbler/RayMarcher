using System;
using System.Numerics;

namespace RayMarchLib
{
    public class Capsule : RMObject
    {
        public float Length { get; set; }
        public float Radius { get; set; }

        public Capsule()
        {
            Length = 1.0f;
            Radius = 0.1f;
        }

        protected override float GetDist(Vector3 v)
        {
            v.Y -= Utils.Clamp(v.Y, -Length / 2.0f, Length / 2.0f);

            return v.Length() - Radius;
        }
    }
}
