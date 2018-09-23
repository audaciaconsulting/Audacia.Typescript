using System;
using System.Collections.Generic;
using System.Linq;
using Audacia.Typescript.Transpiler.Builders;

namespace Audacia.Typescript.Transpiler
{
    public class OutputFile
    {
        public OutputFile(string path) => Path = path;

        public string Path { get; }

        public IEnumerable<Type> Dependencies => Builders.SelectMany(t => t.Dependencies);

        public IEnumerable<Type> IncludedTypes => Builders.Select(t => t.Type);
        
        public IList<Builder> Builders { get; } = new List<Builder>();

        public string Build()
        {
            var file = new TypescriptFile();
            
            foreach (var template in Builders)
                file.Elements.Add(template.Build());

            return file.ToString();
        }
    }
}