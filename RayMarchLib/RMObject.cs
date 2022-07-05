

using System.Numerics;

namespace RayMarchLib
{
    public abstract class RMObject
    {
        public Vector3 Position { get; set; }

        public float Map(Vector3 v)
        {
            return GetDist(v - Position);
        }

        protected abstract float GetDist(Vector3 v);
    }
}
