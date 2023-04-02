using System;
using System.Numerics;
using System.Xml.Linq;

namespace RayMarchLib
{
    public class Light
    {
        public Vector3 Color { get; set; } = Vector3.One;
        public LightType LightType { get; set; }
        public Vector3 Direction { get; set; } = -Vector3.UnitY;
        public Vector3 Position { get; set; }
        /// <summary>
        /// Constant, linear and quadratic attenuation
        /// </summary>
        public Vector3 Attenuation { get; set; } = Vector3.One;
        public float Intensity { get; set; } = 1f;

        public static Light ParseLight(XElement el)
        {
            var light = new Light()
            {
                LightType = Enum.Parse<LightType>(el.Name.LocalName)
            };

            XAttribute attrColor = el.Attribute(nameof(Color));
            if (attrColor is not null)
            {
                light.Color = Utils.ToVec3(attrColor.Value);
            }

            XAttribute attrAttenuation = el.Attribute(nameof(Attenuation));
            if (attrAttenuation is not null)
            {
                light.Attenuation = Utils.ToVec3(attrAttenuation.Value);
            }

            XAttribute attrIntensity = el.Attribute(nameof(Intensity));
            if (attrIntensity is not null)
            {
                light.Intensity = Utils.ParseFloat(attrIntensity.Value);
            }

            switch (light.LightType)
            {
                case LightType.Directional:
                    XAttribute attrDirection = el.Attribute(nameof(Direction));
                    if (attrDirection is not null)
                    {
                        light.Direction = Utils.ToVec3(attrDirection.Value);
                        light.Direction = Vector3.Normalize(light.Direction);
                    }
                    break;

                case LightType.Point:
                    XAttribute attrPosition = el.Attribute(nameof(Position));
                    if (attrPosition is not null)
                    {
                        light.Position = Utils.ToVec3(attrPosition.Value);
                    }
                    break;
            }

            return light;
        }
    }
}
