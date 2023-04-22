using System;
using System.Numerics;

namespace RayMarchLib
{
    public class Cyclinder : Capsule
    {
        protected override float GetDist(Vector3 v)
        {
            Vector2 d = Vector2.Abs(new Vector2(v.XZ().Length(), v.Y)) -
                new Vector2(Radius, Length / 2f);

            return MathF.Max(d.X, d.Y).MinZero() + Vector2.Max(d, Vector2.Zero).Length();
        }
    }
}
