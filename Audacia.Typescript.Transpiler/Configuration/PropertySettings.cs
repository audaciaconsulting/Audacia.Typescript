using System.Xml.Serialization;

namespace Audacia.Typescript.Transpiler.Configuration
{
    public class PropertySettings
    {
        [XmlAttribute("initialize")] public bool Initialize { get; set; } = true;
    }
}