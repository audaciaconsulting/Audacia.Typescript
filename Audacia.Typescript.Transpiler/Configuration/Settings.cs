using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Audacia.Typescript.Transpiler.Builders;

namespace Audacia.Typescript.Transpiler.Configuration
{
    public class Settings
    {
        [XmlElement("Transpile")]
        public List<Transpilation> Outputs { get; set; } = new List<Transpilation>();

        private static readonly XmlSerializer Xml = new XmlSerializer(typeof(Settings));

        public static Settings Load(string path)
        {
            if (!File.Exists(path))
            {
                using (TextWriter writer = new StreamWriter(path))
                    Xml.Serialize(writer, Default);

                throw new InvalidDataException("Failed to find a config file at: "
                    + path + ". A template config file has been automatically generated for you");
            }

            using (var stream = new FileStream(path, FileMode.Open))
                return (Settings) Xml.Deserialize(stream);
        }

        public static Settings Default => new Settings
        {
            Outputs =
            {
                new Transpilation("../../example/output/file.ts")
                {
                    Inputs = new[] {"../../path/to/example/assembly.dll"}.Select(x => new FileBuilder()).ToList()
                },
                new Transpilation("../../example/output/file.ts")
                {
                    Inputs = new[] {"../../another/example/assembly.dll"}.Select(x => new FileBuilder
                    {
                        Namespaces = new[] {"example", "namespace", "specifications "}
                            .Select(y => new NamespaceSettings(y)).ToList()
                    }).ToList()
                }
            }
        };
    }
}