using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Audacia.Typescript.Transpiler.Configuration;
using Audacia.Typescript.Transpiler.Documentation;
using Audacia.Typescript.Transpiler.Extensions;

namespace Audacia.Typescript.Transpiler.Builders
{
    /// <summary>A transpile operation representing the output to a single typescript file.</summary>
    public class FileBuilder
    {
        public string Path => Settings.Path;

        public TypescriptFile Typescript { get; }

        private OutputSettings Settings { get; }

        public FileBuilder(OutputSettings outputSettings, XmlDocumentation documentation)
        {
            Settings = outputSettings;
            Typescript = new TypescriptFile { Path = outputSettings.Path };

            TypeMappings = outputSettings.Inputs
                .Distinct()
                .Select(settings => new { assembly = Assembly.LoadFrom(settings.Assembly), settings })
                .SelectMany(input =>
                {
                    var types = input.assembly.GetTypes().ToArray();

                    types = types.FilterByInputSettings(input.settings);

                    // Filter out subtypes of generics- we only want the top one in the inheritance hierarchy
                    return types.Where(type => !types
                    .Any(other => type.Namespace == other.Namespace
                        && type.Name.Split('`').First() == other.Name.Split('`').First()
                        && other.GetGenericArguments().Length > type.GetGenericArguments().Length))
                    .Select(type => new { type, input.settings });
                })
                .Where(x => x.type.IsClass || x.type.IsInterface || x.type.IsEnum)
                .Where(x => x.type.IsPublic && !x.type.IsNested)
                .Select(x => TypeBuilder.Create(x.type, x.settings, documentation))
                .ToArray();
        }

        public void AddReferences(IEnumerable<FileBuilder> outputFiles)
        {
            var references = outputFiles
                .Except(new[] { this }) // Get files which contain types we depend on
                .Where(mapping => mapping.IncludedTypes.Any(type => Dependencies.Contains(type)));

            foreach (var reference in references)
            {
                var source = new Uri(System.IO.Path.GetFullPath(Path));
                var target = new Uri(System.IO.Path.GetFullPath(reference.Path));
                var relativePath = "./" + source.MakeRelativeUri(target);

                if (relativePath.EndsWith(".ts"))
                    relativePath = relativePath.Substring(0, relativePath.Length - 3);

                var dependencyNames = Dependencies
                    .Where(d => reference.IncludedTypes.Contains(d))
                    .Select(d => d.Name);

                Typescript.Imports.Add(new Import(relativePath, dependencyNames));
            }
        }

        public IEnumerable<Type> Dependencies => TypeMappings.SelectMany(t => t.Dependencies).Distinct();

        public IEnumerable<Type> IncludedTypes => TypeMappings.Select(t => t.SourceType);

        public IReadOnlyCollection<TypeBuilder> TypeMappings { get; }

        public string Build()
        {
            var settings = Transpilation.Settings.CyclicReferences?.Handling ?? CyclicReferenceHandling.Ignore;
            var mappings = TypeMappings.TopologicalSort();

            foreach (var mapping in mappings)
                Typescript.Elements.Add(mapping.Build());

            return Typescript.ToString();
        }
    }
}