using System;
using System.Collections.Generic;
using System.Numerics;
using System.Xml.Linq;

namespace RayMarchLib
{
    public class MengerSponge : RMObject
    {
        public int Iterations { get; set; }

        private static float GetBox(Vector3 v)
        {
            var q = Vector3.Abs(v) - Vector3.One;
            return q.MaxZero().Length() + q.Max().MinZero();
        }

        private static float GetCross(Vector3 v)
        {
            float da = Utils.Max(Vector2.Abs(v.XY()));
            float db = Utils.Max(Vector2.Abs(v.YZ()));
            float dc = Utils.Max(Vector2.Abs(v.ZX()));

            return Utils.Min(da, db, dc) - 1f;
        }

        protected override float GetDist(Vector3 v)
        {
            float d = GetBox(v);

            float s = 1f;
            for (int i = 0; i < Iterations; ++i)
            {
                s *= 3f;

                float c = GetCross(v * s) / s;

                d = MathF.Max(d, -c);

                v = Utils.Mod(v, 2f / s);
            }

            return d;
        }

        public override void Deserialize(Dictionary<string, Material> materials, XElement elObj)
        {
            base.Deserialize(materials, elObj);

            XAttribute attrIterations = elObj.Attribute(nameof(Iterations));
            if (attrIterations is not null)
            {
                Iterations = (int)attrIterations;

                if (Iterations <= 0)
                {
                    throw new SceneDeserializationException("Iterations must be positive");
                }
            }
        }
    }
}
