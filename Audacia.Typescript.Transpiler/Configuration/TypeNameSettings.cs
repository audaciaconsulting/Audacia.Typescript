using System.Xml.Serialization;

namespace Audacia.Typescript.Transpiler.Configuration
{
    public class TypeNameSettings
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}
