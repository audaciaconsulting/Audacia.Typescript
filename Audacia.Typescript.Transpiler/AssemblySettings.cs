using System.Xml.Serialization;

namespace Audacia.Typescript.Transpiler
{
    public class AssemblySettings
    {
        public string Assembly { get; set; }
		
        [XmlArrayItem("Namespace")]
        public string[] Namespaces { get; set; }

        public string Output { get; set; }
    }
}