using System;
using System.Drawing;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
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
            float x = ParseFloat(s[0..ind]);
            x = ToRadians(x);
            int ind2 = s.IndexOf(' ', ind + 1);
            float y = ParseFloat(s[(ind + 1)..ind2]),
                  z = ParseFloat(s[(ind2 + 1)..]);

            y = ToRadians(y);
            z = ToRadians(z);

            return new Vector3(x, y, z);
        }

        public static Vector3 ToVec3(string s)
        {
            int ind = s.IndexOf(' ');
            float x = ParseFloat(s[0..ind]);
            int ind2 = s.IndexOf(' ', ind + 1);
            float y = ParseFloat(s[(ind + 1)..ind2]),
                  z = ParseFloat(s[(ind2 + 1)..]);

            return new Vector3(x, y, z);
        }

        public static Color ToColor(this Vector3 c)
        {
            var v = Vector3.Clamp(c, Vector3.Zero, Vector3.One);

            return Color.FromArgb((int)(v.X * 255f), (int)(v.Y * 255f), (int)(v.Z * 255f));
        }

        public static float ParseFloat(string s)
        {
            return float.Parse(s, CultureInfo.InvariantCulture);
        }

        public static float Sign(float x)
        {
            if (x > 0f)
            {
                return 1f;
            }
            else if (x < 0f)
            {
                return -1f;
            }
            else
            {
                return x;
            }
        }

        public static float Min(float a, float b, float c)
        {
            return MathF.Min(a, MathF.Min(b, c));
        }

        public static float Max(float a, float b, float c)
        {
            return MathF.Max(a, MathF.Max(b, c));
        }

        public static float MaxZero(this float x) => MathF.Max(x, 0f);
        public static float MinZero(this float x) => MathF.Min(x, 0f);

        #region Vectors

        public static Vector3 Pow(Vector3 v, float x) => new(MathF.Pow(v.X, x),
                MathF.Pow(v.Y, x),
                MathF.Pow(v.Z, x));

        public static Vector3 Min(Vector3 a, Vector3 b, Vector3 c)
        {
            return Vector3.Min(a, Vector3.Min(b, c));
        }

        public static Vector3 Max(Vector3 a, Vector3 b, Vector3 c)
        {
            return Vector3.Max(a, Vector3.Max(b, c));
        }

        public static Vector2 XY(this Vector3 a) => new(a.X, a.Y);
        public static Vector2 YX(this Vector3 a) => new(a.Y, a.X);
        public static Vector2 XZ(this Vector3 a) => new(a.X, a.Z);
        public static Vector2 ZX(this Vector3 a) => new(a.Z, a.X);
        public static Vector2 YZ(this Vector3 a) => new(a.Y, a.Z);
        public static Vector2 ZY(this Vector3 a) => new(a.Z, a.Y);
        public static float Max(this Vector3 a) => Max(a.X, a.Y, a.Z);
        public static float Min(this Vector3 a) => Min(a.X, a.Y, a.Z);
        public static Vector3 MaxZero(this Vector3 a) => Vector3.Max(a, Vector3.Zero);
        public static Vector3 MinZero(this Vector3 a) => Vector3.Min(a, Vector3.Zero);

        #endregion
    }
}
