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


        /// <summary>
        /// Based on the provided <see cref="InputSettings"/>, filter by namespaces and specified type names if necessary
        /// </summary>
        /// <param name="types"></param>
        /// <param name="inputSettings"></param>
        /// <returns></returns>
        public static Type[] FilterByInputSettings(this IEnumerable<Type> types, InputSettings inputSettings)
        {
            var namespaceSettings = inputSettings.Namespaces;
            if (namespaceSettings != null)
            {
                types = types.Where(t => namespaceSettings.Any(n => n.Name == t.Namespace));

                var nameSpacesWithoutTypes = namespaceSettings
                    .Where(n => n.Types == null || !n.Types.Any())
                    .Select(n => n.Name);
                var specifiedTypes = namespaceSettings
                    .Where(n => n.Types != null)
                    .SelectMany(n => n.Types)
                    .ToList();

                //If user's specified types within a namespace, only return those types
                if (specifiedTypes.Any())
                {
                    types = types
                        .Where(t => specifiedTypes
                            .Any(type => nameSpacesWithoutTypes.Contains(t.Namespace) || type.Name == t.Name));
                }
            }

            return types.ToArray();
        }
    }
}