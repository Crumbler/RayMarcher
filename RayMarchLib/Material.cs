using System.Numerics;
using System.Xml.Linq;

namespace RayMarchLib
{
    public struct Material
    {
        public Vector3 Color { get; set; }
        public float Ambient { get; set; }
        public float Diffuse { get; set; }
        public float Specular { get; set; }
        public float Shininess { get; set; }

        public Material()
        {
            Color = Vector3.One;
            Ambient = 0.05f;
            Diffuse = 0.1f;
            Specular = 1f;
            Shininess = 64f;
        }

        public static Material
            Default = new(),
            Background = new()
            {
                Color = Vector3.Zero
            };

        #region Operators

        public static Material operator +(Material a, Material b) => new()
        {
            Color = a.Color + b.Color,
            Ambient = a.Ambient + b.Ambient,
            Diffuse = a.Diffuse + b.Diffuse,
            Specular = a.Specular + b.Specular,
            Shininess = a.Shininess + b.Shininess
        };

        public static Material operator *(Material a, float f)
        {
            a.Color *= f;
            a.Ambient *= f;
            a.Diffuse *= f;
            a.Specular *= f;
            a.Shininess *= f;

            return a;
        }

        #endregion

        public static Material ParseMaterial(XElement el)
        {
            var m = new Material();

            XAttribute attrColor = el.Attribute(nameof(Color));
            if (attrColor is not null)
            {
                m.Color = Utils.ToVec3(attrColor.Value);

                if (Vector3.Clamp(m.Color, Vector3.Zero, Vector3.One) != m.Color)
                {
                    throw new SceneDeserializationException($"Color must be between {Vector3.Zero} and {Vector3.One}");
                }
            }

            XAttribute attrAmbient = el.Attribute(nameof(Ambient));
            if (attrAmbient is not null)
            {
                m.Ambient = Utils.ParseFloat(attrAmbient.Value);

                if (m.Ambient < 0f)
                {
                    throw new SceneDeserializationException("Ambient cannot be negative");
                }
            }

            XAttribute attrDiffuse = el.Attribute(nameof(Diffuse));
            if (attrDiffuse is not null)
            {
                m.Diffuse = Utils.ParseFloat(attrDiffuse.Value);

                if (m.Diffuse < 0f)
                {
                    throw new SceneDeserializationException("Diffuse cannot be negative");
                }
            }

            XAttribute attrSpecular = el.Attribute(nameof(Specular));
            if (attrSpecular is not null)
            {
                m.Specular = Utils.ParseFloat(attrSpecular.Value);

                if (m.Specular < 0f)
                {
                    throw new SceneDeserializationException("Specular cannot be negative");
                }
            }

            XAttribute attrShininess = el.Attribute(nameof(Shininess));
            if (attrShininess is not null)
            {
                m.Shininess = Utils.ParseFloat(attrShininess.Value);

                if (m.Shininess < 0f)
                {
                    throw new SceneDeserializationException("Shininess cannot be negative");
                }
            }

            return m;
        }
    }
}
