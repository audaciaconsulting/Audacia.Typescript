using System.Xml.Serialization;

namespace Audacia.Typescript.Transpiler.Configuration
{
    public class CyclicReferencesSettings
    {
        [XmlAttribute("feedback")]
        public FeedbackLevel Feedback { get; set; }
    }
}