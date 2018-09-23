using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace Audacia.Typescript.Transpiler.Documentation
{
    [XmlRoot("members")]
    public class AssemblyDocumentation
    {
        [XmlElement("member")] public MemberDocumentation[] Items { get; set; }

        public static AssemblyDocumentation Load(string assembly)
        {
            var path = assembly.Replace(".dll", ".xml");

            if (!File.Exists(path)) return null;
            using (var reader = new XmlTextReader(path))
            {
                reader.ReadToDescendant("members");
                var deserializer = new XmlSerializer(typeof(AssemblyDocumentation));
                return (AssemblyDocumentation) deserializer.Deserialize(reader.ReadSubtree());
            }
        }

        public MemberDocumentation Class(Type @class)
        {
            return Items.SingleOrDefault(i => i.Name.Replace("T:", string.Empty) == @class.FullName);
        }

        public MemberDocumentation Member(MemberInfo member)
        {
            var name = member.DeclaringType.FullName + '.' + member.Name;
            return Items.SingleOrDefault(i => i.Name.Replace("T:", string.Empty) == name);
        }
    }
}