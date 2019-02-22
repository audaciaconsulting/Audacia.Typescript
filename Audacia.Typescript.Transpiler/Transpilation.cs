using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Audacia.Typescript.Transpiler.Builders;
using Audacia.Typescript.Transpiler.Configuration;
using Audacia.Typescript.Transpiler.Extensions;
using static System.Console;

namespace Audacia.Typescript.Transpiler
{
    /// <summary>Represents a single transpiler task and contains the main entry point for the program.</summary>
    public class Transpilation
    {
        public Transpilation() { }

        public Transpilation(string path) => Path = path;

        [XmlAttribute("path")] public string Path { get; set; }

        [XmlElement("Enums")] public EnumSettings EnumSettings { get; set; }

        [XmlElement("PropertyHandling")] public PropertySettings Properties { get; set; } = new PropertySettings();

        [XmlElement("Assembly")] public List<FileBuilder> Inputs { get; set; } = new List<FileBuilder>();

        [XmlElement("DependencyResolution")]
        public DependencyResolutionSettings DependencyResolution { get; set; }

        [XmlIgnore] private readonly Stopwatch _stopwatch = new Stopwatch();

        public void Transpile()
        {
            _stopwatch.Start();

            foreach (var builder in Inputs)
                builder.Build(this);

            var includedTypes = Inputs.SelectMany(o => o.IncludedTypes);

            var missingTypes = Inputs.SelectMany(o => o.Dependencies)
                .Declarations()
                .Where(type => !Primitive.CanWrite(type))
                .Where(type => type.IsGenericType
                    ? !includedTypes.Contains(type.GetGenericTypeDefinition())
                    : !includedTypes.Contains(type))
                .ToList();

            var count = -1;

            foreach (var type in missingTypes)
                WriteLine("including: " + type.Namespace + "." + type.Name);

            while (missingTypes.Count != count)
            {
                count = missingTypes.Count;
                var dependencies = missingTypes
                    .SelectMany(t => t.Dependencies())
                    .Where(t => !missingTypes.Contains(t))
                    .Declarations()
                    .Where(type => !Primitive.CanWrite(type))
                    .ToList();

                foreach (var dependency in dependencies)
                {
                    WriteLine("including: " + dependency.Namespace + "." + dependency.Name.SanitizeTypeName());
                    missingTypes.Add(dependency);
                }
            }

            foreach (var group in missingTypes.Distinct().GroupBy(type => type.Assembly))
            {
                var output = new FileBuilder { Assembly = group.Key };
                foreach (var type in group) output.Types.Add(type);

                output.Build(this);
                Inputs.Add(output);
            }

            foreach (var builder in Inputs)
            {
                builder.AddReferences(Inputs);

                File.WriteAllText(builder.File.Path, builder.File.ToString());

                ForegroundColor = ConsoleColor.Green;
                WriteLine();
                WriteLine($"Typescript file \"{System.IO.Path.GetFullPath(builder.File.Path)}\" written.");
                WriteLine();
                ResetColor();
            }

            ForegroundColor = ConsoleColor.Green;
            WriteLine($"Typescript transpile completed in {_stopwatch.ElapsedMilliseconds}ms.");
            ResetColor();
        }

        public static void Main(string[] args)
        {
            WriteLine();
            if (!args.Any()) throw new ArgumentException("Please specify the config file location");

            var configFileLocation = args.First();
            var context = Settings.Load(configFileLocation);

            foreach (var output in context.Outputs)
            {
                output.Transpile();
            }
        }
    }
}