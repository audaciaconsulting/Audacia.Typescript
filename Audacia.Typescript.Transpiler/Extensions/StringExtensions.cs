using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Audacia.Typescript.Transpiler.Mappings;

namespace Audacia.Typescript.Transpiler.Extensions
{
    public static class StringExtensions
    {
        public static string CamelCase(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            if (s.Length < 2) return s.ToLowerInvariant();
            return char.ToLowerInvariant(s[0]) + s.Substring(1);
        }
    }
    
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
                    var hasDependencies = mappings.Any(m => element.Dependencies.Contains(m.Type));
                    if (hasDependencies)
                    {
                        Console.WriteLine("wait for dependencies: " + element.Type.Name);
                        continue;
                    }

                    yield return element;
                    removed.Add(element);
                    Console.WriteLine("no dependencies: " + element.Type.Name);
                }

                if (!removed.Any()) throw new InvalidDataException("Cyclic reference detected");
                
                foreach (var mapping in removed)
                    mappings.Remove(mapping);
                
                removed.Clear();
            }
        }
    }
}