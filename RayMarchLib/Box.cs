using System;
using System.Numerics;

namespace RayMarchLib
{
    public class Box : RMObject
    {
        public Vector3 Size { get; set; }

        public Box()
        {
            Size = new Vector3(0.5f);
        }

        protected override float GetDist(Vector3 v)
        {
            var q = Vector3.Abs(v) - Size;
            return Vector3.Max(q, Vector3.Zero).Length() + 
                MathF.Min(MathF.Max(q.X, Math.Max(q.Y, q.Z)), 0.0f);
        }
    }
}
