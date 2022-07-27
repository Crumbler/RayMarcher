

using System.Numerics;
using System.Xml.Serialization;

namespace RayMarchLib
{
    public abstract class RMObject
    {
        public Vector3 Position { get; set; }
        public float Scale { get; set; } = 1.0f;

        /// <summary>
        /// The X, Y and Z components correspond to the Yaw, Pitch and Roll
        /// </summary>
        public Vector3 Rotation { get; set; }

        [XmlIgnore]
        private Matrix4x4 InvModelMatrix { get; set; }

        public string Name { get; set; }

        public float Map(Vector3 v)
        {
            v = Vector3.Transform(v, InvModelMatrix);

            return GetDist(v) * Scale;
        }

        public void PreCalculate()
        {
            var m = Matrix4x4.CreateScale(Scale) * 
                    Matrix4x4.CreateFromYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z) * 
                    Matrix4x4.CreateTranslation(Position);

            Matrix4x4.Invert(m, out m);

            InvModelMatrix = m;
        }

        protected abstract float GetDist(Vector3 v);

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return GetType().Name;
            }

            return Name;
        }
    }
}
