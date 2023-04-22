using System.Numerics;

namespace RayMarchLib
{
    public class Plane : RMObject
    {
        protected override float GetDist(Vector3 v) => v.Y;
    }
}
