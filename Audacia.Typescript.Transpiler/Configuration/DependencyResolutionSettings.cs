using System.Xml.Serialization;

namespace Audacia.Typescript.Transpiler.Configuration
{
    public class DependencyResolutionSettings
    {
        [XmlAttribute("unknownTypes")] public string UnknownTypes { get; set; }
    }
}