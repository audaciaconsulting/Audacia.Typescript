using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Audacia.Typescript.Transpiler.Builders;
using Audacia.Typescript.Transpiler.Configuration;

namespace Audacia.Typescript.Transpiler.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<TypeBuilder> TopologicalSort(this IEnumerable<TypeBuilder> nodes)
        {
            var mappings = nodes.ToList();
            var removed = new HashSet<TypeBuilder>();
            
            while (mappings.Count > 0)
            {
                foreach (var element in mappings)
                {
                    if (mappings.Any(m => element.Inherits == m.SourceType)) continue;

                    yield return element;
                    removed.Add(element);
                }

                if (!removed.Any())
                {
                    var setting = Transpilation.Settings.CyclicReferences?.Feedback ?? FeedbackLevel.Ignore;

                    if (setting == FeedbackLevel.Ignore)
                    {
                        mappings.Clear();
                        break;
                    }
                    if (setting == FeedbackLevel.Error)
                    {
                        var rn = Environment.NewLine;
                        var mappingsList = string.Join(", " + rn, mappings.Select(m => m.SourceType.TypescriptName()));
                        throw new InvalidDataException("Cyclic references detected: " + rn + mappingsList);
                    }
                }
                
                foreach (var mapping in removed)
                    mappings.Remove(mapping);
                
                removed.Clear();
            }
        }
    }
}