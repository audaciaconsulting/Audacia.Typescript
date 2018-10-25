using System.Xml.Serialization;

namespace Audacia.Typescript.Transpiler.Configuration
{
    public class EnumSettings
    {

        [XmlAttribute("valueType")]
        public EnumValueType ValueType { get; set; }
    }


    public enum EnumValueType
    {
        Number,
        String
    }

}
