using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Audacia.Typescript.Transpiler.Configuration;
using Audacia.Typescript.Transpiler.Documentation;
using Audacia.Typescript.Transpiler.Extensions;

namespace Audacia.Typescript.Transpiler.Builders
{
    /// <summary>A transpile operation representing the output to a single typescript file.</summary>
    public class FileBuilder
    {
        [XmlAttribute("name")]
        public string AssemblyPath
        {
            get => Assembly?.Location;
            set => Assembly = Assembly.LoadFrom(value);
        }

        [XmlIgnore] public Assembly Assembly { get; set; }

        [XmlElement("Namespace")] public List<NamespaceSettings> Namespaces { get; set; } = new List<NamespaceSettings>();

        [XmlIgnore] public IList<Type> Types { get; } = new List<Type>();

        [XmlIgnore] public IReadOnlyCollection<TypeBuilder> TypeMappings { get; private set; }

        [XmlIgnore] public TypescriptFile File { get; set; }

        public void AddReferences(IEnumerable<FileBuilder> outputFiles)
        {
            var references = outputFiles
                .Except(new[] { this }) // Get files which contain types we depend on
                .Where(mapping => mapping.IncludedTypes.Any(type => Dependencies.Contains(type)));

            foreach (var reference in references)
            {
                var source = new Uri(Path.GetFullPath(File.Path));
                var target = new Uri(Path.GetFullPath(reference.File.Path));
                var relativePath = "./" + source.MakeRelativeUri(target);

                if (relativePath.EndsWith(".ts"))
                    relativePath = relativePath.Substring(0, relativePath.Length - 3);

                var includedNames = reference.IncludedTypes.Select(x => x.FullName.SanitizeTypeName());
                var dependencyNames = Dependencies // Compare by full name so we include generics.
                    .Where(d => includedNames.Contains(d.FullName.SanitizeTypeName()))
                    .Select(d => d.Name.SanitizeTypeName());

                File.Imports.Add(new Import(relativePath, dependencyNames));
            }
        }

        public IEnumerable<Type> Dependencies => TypeMappings?.SelectMany(t => t.Dependencies)
            .Where(result => result != null)
            .Where(result => result.FullName != null)
            .DistinctBy(result => result.FullName.SanitizeTypeName());

        public IEnumerable<Type> IncludedTypes => TypeMappings?.Select(t => t.SourceType);

        public string Build(Transpilation context)
        {
            if (Assembly == null)
                throw new InvalidDataException("Please specify an assembly.");

            this.Documentation = XmlDocumentation.Load(AssemblyPath);

            var name = string.Join(".", Assembly.GetName().Name
                           .TrimEnd()
                           .Split('.')
                           .Select(s => s.CamelCase()))
                       + ".ts";

            File = new TypescriptFile { Path = Path.Combine(context.Path, name) };

            TypeMappings = Assembly.GetTypes().Declarations()
                .Where(type => type.IsPublic && !type.IsNested)
                .Select(type => TypeBuilder.Create(type, this, context))
                .Where(m => Types.Contains(m.SourceType) || !Types.Any())
                .TopologicalSort()
                .ToArray();

            File.Elements.Add(new Comment("This file is generated from Audacia.Typescript.Transpiler. Any changes will be overwritten. \n"));

            foreach (var mapping in TypeMappings)
                File.Elements.Add(mapping.Build());

            return File.ToString();
        }

        public XmlDocumentation Documentation { get; set; }
    }
}