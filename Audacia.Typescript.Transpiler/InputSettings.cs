using System.Xml.Serialization;

namespace Audacia.Typescript.Transpiler
{
    public class InputSettings
    {
        public InputSettings() { }
        
        public InputSettings(string assembly) => Assembly = assembly;

        [XmlAttribute("name")]
        public string Assembly { get; set; }
		
        [XmlElement("Namespace")]
        public NamespaceSettings[] Namespaces { get; set; }
    }

    public class NamespaceSettings
    {
        public NamespaceSettings() { }

        public NamespaceSettings(string name) => Name = name;

        [XmlAttribute("name")]
        public string Name { get; set; }
    }
    
    public class OutputSettings
    {
        public OutputSettings() { }
        
        public OutputSettings(string path) => Path = path;

        [XmlAttribute("path")]
        public string Path { get; set; }
        
        [XmlElement("Input")]
        public InputSettings[] Inputs { get; set; }
    }
}