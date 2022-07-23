

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

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

        private static readonly XmlSerializer serializer;
        private static readonly Type[] objectTypes;
        private static readonly XmlWriterSettings writerSettings = new()
        {
            Indent = true
        };
           

        static Scene()
        {
            objectTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(RMObject)))
                .ToArray();

            serializer = new XmlSerializer(typeof(Scene), objectTypes);
        }

        public void SerializeToFile(string fileName)
        {
            using var writer = XmlWriter.Create(fileName, writerSettings);

            serializer.Serialize(writer, this);
        }

        public static Scene LoadFromFile(string fileName)
        {
            var scene = serializer.Deserialize(XmlReader.Create(fileName)) as Scene;

            return scene;
        }

        public override string ToString()
        {
            return nameof(Scene);
        }
    }
}
