using System;
using System.Numerics;

namespace RayMarchLib
{
    public class Torus : RMObject
    {
        public float InnerRadius { get; set; }
        public float OuterRadius { get; set; }

        public Torus()
        {
            InnerRadius = 0.5f;
            OuterRadius = 0.1f;
        }

        protected override float GetDist(Vector3 v)
        {
            var q = new Vector2(new Vector2(v.X, v.Z).Length() - InnerRadius, v.Y);

            return q.Length() - OuterRadius;
        }
    }
}
