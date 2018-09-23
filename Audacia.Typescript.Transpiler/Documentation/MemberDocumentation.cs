using System.Xml.Serialization;

namespace Audacia.Typescript.Transpiler.Documentation
{
    public class MemberDocumentation
    {
        private string _summary;

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("summary")]
        public string Summary
        {
            get => _summary;
            set => _summary = value;
        }

        [XmlElement("example")]
        public string Example { get; set; }

        [XmlElement("remarks")]
        public string Remarks { get; set; }
    }
}