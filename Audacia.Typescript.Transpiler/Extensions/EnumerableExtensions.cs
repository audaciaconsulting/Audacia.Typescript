using System;
using System.Collections.Generic;
using System.Linq;
using Audacia.Typescript.Transpiler.Builders;

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
                foreach (var element in mappings.OrderBy(m => !(m is EnumBuilder)))
                {
                    if (mappings.Any(m => element.Inherits == m.SourceType)) continue;
                    if (mappings.Any(m => element.ClassAttributeDependencies.Contains(m.SourceType))) continue;
                    if (mappings.Any(m => element.PropertyAttributeDependencies.Contains(m.SourceType))) continue;

                    yield return element;
                    removed.Add(element);
                }

                foreach (var mapping in removed)
                    mappings.Remove(mapping);

                removed.Clear();
            }
        }

        public static IEnumerable<Type> Flatten(this Type source)
        {
            var result = new List<Type>();
            
            if (!source.IsGenericType)
                result.Add(source);
            else foreach (var argument in source.GetGenericArguments().Where(a => !a.IsGenericParameter))
                result.AddRange(argument.Flatten());

            return result;
        }

        /// <summary>
        /// Based on the provided <see cref="FileBuilder"/>, filter by namespaces and specified type names if necessary
        /// </summary>
        /// <param name="types"></param>
        /// <param name="inputSettings"></param>
        /// <returns></returns>
        public static Type[] FilterBy(this IEnumerable<Type> types, FileBuilder inputSettings)
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