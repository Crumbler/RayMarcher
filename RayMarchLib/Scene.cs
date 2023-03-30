using System;
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

        public Dictionary<string, Material> Materials { get; private set; }

        public Scene()
        {
            ImageWidth = 100;
            ImageHeight = 100;
            Fov = MathF.PI / 2.0f;
            Eps = 0.001f;
            MaxDist = 100.0f;
            MaxIterations = 80;

            Objects = new List<RMObject>();
            Materials = new Dictionary<string, Material>();
        }

        public static Scene LoadFromFile(string fileName)
        {
            var doc = XDocument.Load(fileName);

            var scene = new Scene();

            XElement elScene = doc.Root;

            scene.ImageWidth = (int)elScene.Element(nameof(ImageWidth));
            scene.ImageHeight = (int)elScene.Element(nameof(ImageHeight));
            scene.Fov = Utils.ToRadians(Utils.ParseFloat(elScene.Element(nameof(Fov)).Value));
            scene.Eps = Utils.ParseFloat(elScene.Element(nameof(Eps)).Value);
            scene.MaxDist = (int)elScene.Element(nameof(MaxDist));
            scene.MaxIterations = (int)elScene.Element(nameof(MaxIterations));

            XElement elMaterials = elScene.Element(nameof(Materials));

            if (elMaterials is not null)
            {
                foreach (XElement elMaterial in elMaterials.Descendants())
                {
                    Material m = Material.ParseMaterial(elMaterial);

                    scene.Materials.Add(elMaterial.Name.LocalName, m);
                }
            }

            XElement elObjects = elScene.Element(nameof(Objects));

            foreach (XElement elObj in elObjects.Descendants())
            {
                Type objType = Type.GetType($"RayMarchLib.{elObj.Name}", true);

                RMObject obj = (RMObject)Activator.CreateInstance(objType);

                obj.Deserialize(scene.Materials, elObj);

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
