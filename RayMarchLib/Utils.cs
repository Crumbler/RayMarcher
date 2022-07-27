

namespace RayMarchLib
{
    public static class Utils
    {
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
    }
}
