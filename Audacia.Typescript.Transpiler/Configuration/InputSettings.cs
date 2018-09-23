using System.Xml.Serialization;

namespace Audacia.Typescript.Transpiler.Configuration
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
}