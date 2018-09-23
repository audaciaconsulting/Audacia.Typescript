using System;
using System.Collections.Generic;
using System.Linq;

namespace Audacia.Typescript
{
    public class Import : Element
    {
        public Import() { }
        
        public Import(string path) => Path = path;
        
        public Import(string path, IEnumerable<string> types) : this (path) => Types = types.ToList();

        public string Path { get; set; }

        public IList<string> Types { get; set; } = new List<string>();
        
        public override TypescriptBuilder Build(TypescriptBuilder builder, IElement parent)
        {
            if (!Types.Any()) return builder;
            
            builder.Append("import {");

            if (Types.Count == 1) builder.Append(' ').Append(Types.Single()).Append(" }");
            else builder.Indent().NewLine().Join(Types, ',' + Environment.NewLine).Append(" }").Unindent();

            return builder.Append(" from '").Append(Path).Append("';");
        }
    }
}