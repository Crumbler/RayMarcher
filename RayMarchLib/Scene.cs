using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

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
        public List<RMObject> Objects { get; private set; }

        public Dictionary<int, Material> Materials { get; private set; }

        public Scene()
        {
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

        private static readonly XmlWriterSettings writerSettings = new()
        {
            Indent = true
        };

        public void SerializeToFile(string fileName)
        {
            var doc = new XDocument();

            var elScene = new XElement(nameof(Scene));

            doc.Add(elScene);

            elScene.Add(new XElement(nameof(Fov), Utils.ToDegrees(Fov).ToString(CultureInfo.InvariantCulture)),
                new XElement(nameof(Eps), Eps.ToString(CultureInfo.InvariantCulture)),
                new XElement(nameof(MaxDist), MaxDist.ToString(CultureInfo.InvariantCulture)),
                new XElement(nameof(MaxIterations), MaxIterations.ToString(CultureInfo.InvariantCulture)));

            var elObjects = new XElement(nameof(Objects));

            elScene.Add(elObjects);

            for (int i = 0; i < Objects.Count; ++i)
            {
                RMObject obj = Objects[i];
                var elObj = new XElement("Obj");

                obj.Serialize(elObj);

                elObjects.Add(elObj);
            }

            doc.Save(fileName);
        }

        public static Scene LoadFromFile(string fileName)
        {
            var doc = XDocument.Load(fileName);

            var scene = new Scene();

            XElement elScene = doc.Root ?? Utils.XEmpty;
            
            scene.Fov = Utils.ToRadians((float)elScene.Element(nameof(Fov)));
            scene.Eps = (float)elScene.Element(nameof(Eps));
            scene.MaxDist = (int)elScene.Element(nameof(MaxDist));
            scene.MaxIterations = (int)elScene.Element(nameof(MaxIterations));

            XElement elObjects = elScene.Element(nameof(Objects)) ?? Utils.XEmpty;

            foreach (XElement elObj in elObjects.Descendants())
            {
                if (elObj.Name == nameof(Sphere))
                {
                    Sphere sph = new();

                    sph.Deserialize(elObj);

                    scene.Objects.Add(sph);
                }
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

        public override string ToString()
        {
            return nameof(Scene);
        }
    }
}
