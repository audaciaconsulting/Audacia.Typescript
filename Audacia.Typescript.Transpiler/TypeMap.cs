using System;
using System.Collections.Generic;
using System.Linq;
using Audacia.Typescript.Transpiler.Builders;

namespace Audacia.Typescript.Transpiler
{
    public class TypeMap
    {
        private readonly IDictionary<Type, TypeBuilder[]> _mappings;

        public bool ContainsType(Type type) => _mappings.ContainsKey(type);
        
        public TypeBuilder[] this[Type type] => _mappings[type];

        public List<TypescriptFile> Files { get; set; }
        
        public TypeMap(IEnumerable<FileBuilder> fileMappings)
        {
            _mappings = fileMappings.SelectMany(f => f.TypeMappings)
                .GroupBy(m => m.SourceType)
                .ToDictionary(g => g.Key, g => g.ToArray());
        }
    }
}