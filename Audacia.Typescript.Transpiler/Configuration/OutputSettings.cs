using System.Collections.Generic;
using System.Xml.Serialization;

namespace Audacia.Typescript.Transpiler.Configuration
{
    public class OutputSettings
    {
        public OutputSettings() { }

        public OutputSettings(string path) => Path = path;

        [XmlAttribute("path")]
        public string Path { get; set; }

        [XmlElement("Input")]
        public List<InputSettings> Inputs { get; set; } = new List<InputSettings>();
    }
}