using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Audacia.Typescript.Transpiler.Extensions;

namespace Audacia.Typescript.Transpiler.Documentation
{
    public class XmlDocumentation
    {
        private readonly Dictionary<string, AssemblyDocumentation> _documentation =
            new Dictionary<string, AssemblyDocumentation>();

        public static XmlDocumentation Load(IEnumerable<string> assemblies)
        {
            var result = new XmlDocumentation();

            foreach (var assembly in assemblies)
            {
                try
                {
                    var docs = AssemblyDocumentation.Load(assembly);
                    if (docs != null) result._documentation.Add(assembly, docs);
                }
                catch (InvalidOperationException e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine();
                    Console.WriteLine("Warning: Failed to read XML Documentation file for assembly: " + assembly);

                    if (e.InnerException is XmlException x &&
                        x.Message.EndsWith("'Element' is an invalid XmlNodeType."))
                    {
                        Console.WriteLine();
                        Console.WriteLine("This is most likely due to the presence of <cref> attributes in your documentation tags, which are not supported by the .NET XML Deserializer.");
                        Console.WriteLine();
                    }
                    
                    Console.WriteLine(e);
                    Console.WriteLine();
                }
            }

            return result;
        }

        public MemberDocumentation ForClass(Type @class)
        {
            foreach (var documentation in _documentation)
            {
                var result = documentation.Value.Class(@class);
                if (result != null) return result;
            }

            return null;
        }

        public MemberDocumentation ForMember(MemberInfo member)
        {
            foreach (var documentation in _documentation)
            {
                var result = documentation.Value.Member(member);
                if (result != null) return result;
            }

            return null;
        }
    }
}