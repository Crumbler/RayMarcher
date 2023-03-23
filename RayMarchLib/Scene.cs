﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace RayMarchLib
{
    public class Scene
    {
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
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
        public List<RMObject> Objects { get; private set; }

        public Dictionary<int, Material> Materials { get; private set; }

        public Scene()
        {
            ImageWidth = 100;
            ImageHeight = 100;
            Fov = MathF.PI / 2.0f;
            Eps = 0.001f;
            MaxDist = 100.0f;
            MaxIterations = 80;

            Objects = new List<RMObject>();
            Materials = new Dictionary<int, Material>()
            {
                { -1, Material.Default }
            };
        }

        public static Scene LoadFromFile(string fileName)
        {
            var doc = XDocument.Load(fileName);

            var scene = new Scene();

            XElement elScene = doc.Root ?? Utils.XEmpty;

            scene.ImageWidth = (int)elScene.Element(nameof(ImageWidth));
            scene.ImageHeight = (int)elScene.Element(nameof(ImageHeight));
            scene.Fov = Utils.ToRadians(float.Parse(elScene.Element(nameof(Fov))!.Value, CultureInfo.InvariantCulture));
            scene.Eps = float.Parse(elScene.Element(nameof(Eps))!.Value, CultureInfo.InvariantCulture);
            scene.MaxDist = (int)elScene.Element(nameof(MaxDist));
            scene.MaxIterations = (int)elScene.Element(nameof(MaxIterations));

            XElement elObjects = elScene.Element(nameof(Objects)) ?? Utils.XEmpty;

            foreach (XElement elObj in elObjects.Descendants())
            {
                Type objType = Type.GetType($"RayMarchLib.{elObj.Name}", true);

                RMObject obj = (RMObject)Activator.CreateInstance(objType);

                obj.Deserialize(elObj);

                scene.Objects.Add(obj);
            }

            return scene;
        }

        public void PreCalculate()
        {
            foreach (RMObject obj in Objects)
            {
                obj.PreCalculate();
            }
        }
    }
}
