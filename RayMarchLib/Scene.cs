﻿using System;
using System.Collections.Generic;
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
        public Camera Camera { get; set; }

        public Dictionary<string, Material> Materials { get; private set; }

        public Scene()
        {
            ImageWidth = 100;
            ImageHeight = 100;
            Fov = MathF.PI / 2.0f;
            Eps = 0.001f;
            MaxDist = float.PositiveInfinity;
            MaxIterations = 80;

            Objects = new List<RMObject>();
            Camera = new Camera();
            Materials = new Dictionary<string, Material>();
        }

        public static Scene LoadFromFile(string fileName)
        {
            var doc = XDocument.Load(fileName);

            var scene = new Scene();

            XElement elScene = doc.Root;

            LoadParams(scene, elScene);

            XElement elMaterials = elScene.Element(nameof(Materials));
            if (elMaterials is not null)
            {
                LoadMaterials(scene, elMaterials);
            }

            XElement elObjects = elScene.Element(nameof(Objects));

            LoadObjects(scene, elObjects);

            return scene;
        }

        private static void LoadParams(Scene scene, XElement elScene)
        {
            scene.ImageWidth = (int)elScene.Element(nameof(ImageWidth));
            scene.ImageHeight = (int)elScene.Element(nameof(ImageHeight));
            scene.Fov = Utils.ToRadians(Utils.ParseFloat(elScene.Element(nameof(Fov)).Value));
            scene.Eps = Utils.ParseFloat(elScene.Element(nameof(Eps)).Value);

            XElement elMaxDist = elScene.Element(nameof(MaxDist));
            if (elMaxDist is not null)
            {
                scene.MaxDist = Utils.ParseFloat(elMaxDist.Value);
            }

            scene.MaxIterations = (int)elScene.Element(nameof(MaxIterations));

            XElement elCamera = elScene.Element(nameof(Camera));
            if (elCamera is not null)
            {
                scene.Camera = Camera.Deserialize(elCamera);
            }
        }

        private static void LoadMaterials(Scene scene, XElement elMaterials)
        {
            foreach (XElement elMaterial in elMaterials.Elements())
            {
                Material m = Material.ParseMaterial(elMaterial);

                scene.Materials.Add(elMaterial.Name.LocalName, m);
            }

            if (scene.Materials.ContainsKey(nameof(Material.Background)))
            {
                Material.Background = scene.Materials[nameof(Material.Background)];
                scene.Materials.Remove(nameof(Material.Background));
            }

            if (scene.Materials.ContainsKey(nameof(Material.Default)))
            {
                Material.Default = scene.Materials[nameof(Material.Default)];
                scene.Materials.Remove(nameof(Material.Default));
            }
        }

        private static void LoadObjects(Scene scene, XElement elObjects)
        {
            Stack<IRMGroup> groups = new();
            Stack<XElement> descObjects = new();

            IRMGroup currGroup = null;

            foreach (XElement elObj in elObjects.Elements())
            {
                descObjects.Push(elObj);
            }

            while (descObjects.Count != 0)
            {
                XElement elObj = descObjects.Pop();

                // Stop deserializing group
                if (elObj is null)
                {
                    // Return to adding to scene
                    if (groups.Count == 0)
                    {
                        currGroup = null;
                    }
                    else
                    {
                        // Return to parent object
                        currGroup = groups.Pop();
                    }

                    continue;
                }

                Type objType = Type.GetType($"RayMarchLib.{elObj.Name}", true);

                RMObject obj = (RMObject)Activator.CreateInstance(objType);

                obj.Deserialize(scene.Materials, elObj);

                if (currGroup is null)
                {
                    scene.Objects.Add(obj);
                }
                else
                {
                    currGroup.AddObject(obj);
                }

                if (obj is IRMGroup group)
                {
                    if (currGroup is not null)
                    {
                        groups.Push(currGroup);
                    }

                    currGroup = group;

                    group.DeserializeGroup(elObj, descObjects);
                }
            }
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
