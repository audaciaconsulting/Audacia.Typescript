using System;
using System.Collections.Generic;
using System.Reflection;

namespace Audacia.Typescript.Transpiler.Documentation
{
    public class XmlDocumentation
    {
        private readonly Dictionary<string, AssemblyDocumentation> _documentation =
            new Dictionary<string, AssemblyDocumentation>();

        public static XmlDocumentation Load(string assembly)
        {
            var result = new XmlDocumentation();

            try
            {
                var docs = AssemblyDocumentation.Load(assembly);
                if (docs != null) result._documentation.Add(assembly, docs);
            }
            catch (InvalidOperationException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine(e);
                Console.WriteLine();
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