using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
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

            var tempPath = ReplaceCrefTags(path);
            
            using (var reader = new XmlTextReader(tempPath))
            {
                reader.ReadToDescendant("members");
                var deserializer = new XmlSerializer(typeof(AssemblyDocumentation));
                return (AssemblyDocumentation) deserializer.Deserialize(reader.ReadSubtree());
            }
        }

        private static string ReplaceCrefTags(string path)
        {
            var content = File.ReadAllText(path);
            
            // todo; <para> tag
            // dirty filthy regexes
            var summaryRegex = new Regex(@"<summary>(.*)<\/summary>");
            var returnsRegex = new Regex(@"<returns>(.*)<\/returns>");
            var crefRegex = new Regex(@"<(c|paramref|see|seealso|typeparamref) cref="".:([^""]*)""\/>");
            var summaryMatches = summaryRegex.Matches(content).Concat(returnsRegex.Matches(content));

            foreach (Match summaryMatch in summaryMatches)
            {
                var originalSummary = summaryMatch.Captures[0].Value;
                var tagMatches = crefRegex.Matches(originalSummary);
                
                var newSummary = originalSummary;
                
                foreach (Match tagMatch in tagMatches)
                {
                    var replacement = tagMatch.Groups[2].Captures.Single().Value
                        .Split('`').First()
                        .Split('.').Last();

                    newSummary = newSummary.Replace(tagMatch.Value, replacement);
                }

                content = content.Replace(originalSummary, newSummary);
            }

            var tempFileName = Path.GetTempFileName();
            File.WriteAllText(tempFileName, content);

            return tempFileName;
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