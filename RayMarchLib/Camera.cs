using System.Numerics;
using System.Xml.Linq;

namespace RayMarchLib
{
    public class Camera
    {
        public Vector3 Position { get; set; }
        public Vector3 Target { get; set; } = new Vector3(0, 0, -1);
        
        public static Camera Deserialize(XElement el)
        {
            var camera = new Camera();

            XAttribute attrPosition = el.Attribute(nameof(Position));
            if (attrPosition is not null)
            {
                camera.Position = Utils.ToVec3(attrPosition.Value);
            }

            XAttribute attrTarget = el.Attribute(nameof(Target));
            if (attrTarget is not null)
            {
                camera.Target = Utils.ToVec3(attrTarget.Value);
            }

            return camera;
        }
    }
}
