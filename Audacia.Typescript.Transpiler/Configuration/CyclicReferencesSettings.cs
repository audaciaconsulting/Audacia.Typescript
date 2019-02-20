using System.Xml.Serialization;

namespace Audacia.Typescript.Transpiler.Configuration
{
    public class CyclicReferencesSettings
    {
        [XmlAttribute("handling")]
        public CyclicReferenceHandling Handling { get; set; }
        
        [XmlAttribute("feedback")]
        public FeedbackLevel Feedback { get; set; }
    }
}