using System;
using System.Globalization;
using System.Numerics;
using System.Xml.Linq;

namespace RayMarchLib
{
    public static class Utils
    {
        public static readonly XElement XEmpty = new(string.Empty);
        public static float Clamp(float x, float a, float b)
        {
            if (x < a)
            {
                return a;
            }

            if (x > b)
            {
                return b;
            }

            return x;
        }

        public static string ToString(Vector3 v)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", v.X, v.Y, v.Z);
        }

        public static string ToDegreesString(Vector3 v)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", 
                                 ToDegrees(v.X), ToDegrees(v.Y), ToDegrees(v.Z));
        }

        public static float ToDegrees(float x)
        {
            return 180.0f * x / MathF.PI;
        }

        public static float ToRadians(float x)
        {
            return x * MathF.PI / 180.0f;
        }

        public static Vector3 ToRadiansVec3(string s)
        {
            int ind = s.IndexOf(' ');
            var x = float.Parse(s[0..ind], CultureInfo.InvariantCulture);
            x = ToRadians(x);
            int ind2 = s.IndexOf(' ', ind + 1);
            float y = float.Parse(s[(ind + 1)..ind2], CultureInfo.InvariantCulture),
                  z = float.Parse(s[(ind2 + 1)..], CultureInfo.InvariantCulture);

            y = ToRadians(y);
            z = ToRadians(z);

            return new Vector3(x, y, z);
        }

        public static Vector3 ToVec3(string s)
        {
            int ind = s.IndexOf(' ');
            float x = float.Parse(s[0..ind], CultureInfo.InvariantCulture);
            int ind2 = s.IndexOf(' ', ind + 1);
            float y = float.Parse(s[(ind + 1)..ind2], CultureInfo.InvariantCulture),
                  z = float.Parse(s[(ind2 + 1)..], CultureInfo.InvariantCulture);

            return new Vector3(x, y, z);
        }
    }
}
