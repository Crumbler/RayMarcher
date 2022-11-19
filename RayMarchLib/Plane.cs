using System.Numerics;
using System.Xml.Linq;

namespace RayMarchLib
{
    public class Plane : RMObject
    {
        protected override float GetDist(Vector3 v) => v.Y;
    }
}
