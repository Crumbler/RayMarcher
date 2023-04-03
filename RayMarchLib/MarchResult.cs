using System.Numerics;

namespace RayMarchLib
{
    public struct MarchResult
    {
        public Vector3 origin, direction, position;
        public float distance, distToObject;
    }
}
