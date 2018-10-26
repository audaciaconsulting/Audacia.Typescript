using System.Xml.Serialization;

namespace Audacia.Typescript.Transpiler.Configuration
{
    public class NamespaceSettings
    {
        public NamespaceSettings() { }

        public NamespaceSettings(string name) => Name = name;

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("TypeName")]
        public TypeNameSettings[] Types { get; set; } = new TypeNameSettings[0];
    }
}