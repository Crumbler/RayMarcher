﻿using System.Numerics;
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

        public static Material ParseMaterial(XElement el)
        {
            var m = new Material();

            XAttribute attrColor = el.Attribute(nameof(Color));
            if (attrColor is not null)
            {
                m.Color = Utils.ToVec3(attrColor.Value);
            }

            XAttribute attrAmbient = el.Attribute(nameof(Ambient));
            if (attrAmbient is not null)
            {
                m.Ambient = Utils.ParseFloat(attrAmbient.Value);
            }

            XAttribute attrDiffuse = el.Attribute(nameof(Diffuse));
            if (attrDiffuse is not null)
            {
                m.Diffuse = Utils.ParseFloat(attrDiffuse.Value);
            }

            XAttribute attrSpecular = el.Attribute(nameof(Specular));
            if (attrSpecular is not null)
            {
                m.Specular = Utils.ParseFloat(attrSpecular.Value);
            }

            XAttribute attrShininess = el.Attribute(nameof(Shininess));
            if (attrShininess is not null)
            {
                m.Shininess = Utils.ParseFloat(attrShininess.Value);
            }

            return m;
        }
    }
}
