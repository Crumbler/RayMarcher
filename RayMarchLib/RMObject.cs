using System.Collections.Generic;
using System.Numerics;
using System.Xml.Linq;

namespace RayMarchLib
{
    public abstract class RMObject
    {
        public Vector3 Position { get; set; }
        public Material? Material { get; set; }
        public float Scale { get; set; } = 1.0f;

        /// <summary>
        /// Rotation around the X, Y and Z axes
        /// </summary>
        public Vector3 Rotation { get; set; }

        private Matrix4x4 InvModelMatrix { get; set; }

        public float Map(Vector3 v)
        {
            v = Vector3.Transform(v, InvModelMatrix);

            return GetDist(v) * Scale;
        }

        public virtual void PreCalculate()
        {
            var m = Matrix4x4.CreateScale(Scale) * 
                    Matrix4x4.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z) * 
                    Matrix4x4.CreateTranslation(Position);

            Matrix4x4.Invert(m, out m);

            InvModelMatrix = m;
        }

        protected abstract float GetDist(Vector3 v);

        public HitResult MapHit(Vector3 v)
        {
            v = Vector3.Transform(v, InvModelMatrix);

            var hit = GetHit(v);
            hit.distance *= Scale;

            return hit;
        }

        protected virtual HitResult GetHit(Vector3 v)
        {
            return new HitResult()
            {
                distance = GetDist(v),
                material = Material
            };
        }

        public virtual void Deserialize(Dictionary<string, Material> materials, XElement elObj)
        {
            XAttribute attrMaterialName = elObj.Attribute(nameof(Material));
            if (attrMaterialName is not null)
            {
                Material = materials[attrMaterialName.Value];
            }

            XAttribute attrPosition = elObj.Attribute(nameof(Position));
            if (attrPosition is not null)
            {
                Position = Utils.ToVec3(attrPosition.Value);
            }

            XAttribute attrRotation = elObj.Attribute(nameof(Rotation));
            if (attrRotation is not null)
            {
                Rotation = Utils.ToRadiansVec3(attrRotation.Value);
            }

            XAttribute attrScale = elObj.Attribute(nameof(Scale));
            if (attrScale is not null)
            {
                Scale = Utils.ParseFloat(attrScale.Value);
            }
        }
    }
}
