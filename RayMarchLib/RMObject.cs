using System.Globalization;
using System.Numerics;
using System.Xml;
using System.Xml.Linq;

namespace RayMarchLib
{
    public abstract class RMObject
    {
        public Vector3 Position { get; set; }
        public int MaterialId { get; set; } = -1;
        public float Scale { get; set; } = 1.0f;

        /// <summary>
        /// The X, Y and Z components correspond to the Yaw, Pitch and Roll
        /// </summary>
        public Vector3 Rotation { get; set; }

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
        public virtual void Serialize(XElement el)
        {
            el.Name = GetType().Name;

            if (!string.IsNullOrWhiteSpace(Name))
            {
                el.Add(new XAttribute(nameof(Name), Name));
            }

            if (MaterialId != -1)
            {
                el.Add(new XAttribute(nameof(MaterialId), MaterialId.ToString(CultureInfo.InvariantCulture)));
            }

            el.Add(new XAttribute(nameof(Scale), Scale.ToString(CultureInfo.InvariantCulture)));
            el.Add(new XAttribute(nameof(Position), Utils.ToString(Position)));
            el.Add(new XAttribute(nameof(Rotation), Utils.ToDegreesString(Rotation)));
        }

        public virtual void Deserialize(XElement elObj)
        {
            XAttribute attrName = elObj.Attribute(nameof(Name));
            if (attrName is not null)
            {
                Name = attrName.Value;
            }

            XAttribute attrMaterialId = elObj.Attribute(nameof(MaterialId));
            if (attrMaterialId is not null)
            {
                MaterialId = (int)attrMaterialId;
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
                Scale = (float)attrScale;
            }
        }

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
