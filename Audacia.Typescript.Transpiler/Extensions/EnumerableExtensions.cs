using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Audacia.Typescript.Transpiler.Mappings;

namespace Audacia.Typescript.Transpiler.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<TypeMapping> TopologicalSort(this IEnumerable<TypeMapping> nodes)
        {
            var mappings = nodes.ToHashSet();
            var removed = new HashSet<TypeMapping>();
            
            while (mappings.Count > 0)
            {
                foreach (var element in mappings)
                {
                    if (mappings.Any(m => element.Dependencies.Contains(m.SourceType))) continue;

                    yield return element;
                    removed.Add(element);
                }

                if (!removed.Any())
                {
                    var mappingsList = string.Join(", ", mappings.Select(m => m.SourceType.TypescriptName()));
                    throw new InvalidDataException("Cyclic reference detected: " + mappingsList);
                }
                
                foreach (var mapping in removed)
                    mappings.Remove(mapping);
                
                removed.Clear();
            }
        }
    }
}