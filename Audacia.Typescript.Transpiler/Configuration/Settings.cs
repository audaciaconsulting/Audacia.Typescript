using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Audacia.Typescript.Transpiler.Configuration
{
    public class DependencyResolutionSettings
    {
        [XmlAttribute("unknownTypes")]
        public string UnknownTypes { get; set; }
    }
    
    public class Settings
    {
        [XmlElement("Output")] 
        public OutputSettings[] Outputs { get; set; }
        
        [XmlElement("DependencyResolution")] 
        public DependencyResolutionSettings DependencyResolution { get; set; }
        
        [XmlElement("CyclicReferences")] 
        public CyclicReferencesSettings CyclicReferences { get; set; }
        
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
            Outputs = new[]
            {
                new OutputSettings("../../example/output/file.ts")
                {
                    Inputs = new[] {"../../path/to/example/assembly.dll"}.Select(x => new InputSettings(x)).ToArray()
                },
                new OutputSettings("../../example/output/file.ts")
                {
                    Inputs = new[] {"../../another/example/assembly.dll"}.Select(x => new InputSettings(x)
                    {
                        Namespaces = new[] {"example", "namespace", "specifications "}
                            .Select(y => new NamespaceSettings(y)).ToArray()
                    }).ToArray()
                }
            }
        };
    }
}