

using System;
using System.Collections.Generic;

namespace RayMarchLib
{
    public class Scene
    {
        /// <summary>
        /// Camera Field of View
        /// </summary>
        public float Fov { get; set; }
        public float Eps { get; set; }
        /// <summary>
        /// The maximum distance after which the iterations stop
        /// </summary>
        public float MaxDist { get; set; }
        public int MaxIterations { get; set; }
        public List<RMObject> Objects { get; set; }

        public Scene()
        {
            Fov = MathF.PI / 2.0f;
            Eps = 0.001f;
            MaxDist = 20.0f;
            MaxIterations = 50;

            Objects = new List<RMObject>();
        }

        public override string ToString()
        {
            return nameof(Scene);
        }
    }
}
