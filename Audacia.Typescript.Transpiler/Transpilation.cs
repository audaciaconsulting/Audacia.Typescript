using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Audacia.Typescript.Transpiler.Builders;
using Audacia.Typescript.Transpiler.Configuration;
using Audacia.Typescript.Transpiler.Extensions;
using Audacia.Typescript.Transpiler.Logging;

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

        public bool Debug { get; set; } = true;

        public void Transpile()
        {
            _stopwatch.Start();

            foreach (var builder in Inputs)
                builder.Build(this);

            var includedTypes = Inputs.SelectMany(o => o.IncludedTypes);

            var missingTypes = Inputs.SelectMany(o => o.Dependencies)
                .Concat(Inputs.SelectMany(i => i.ClassAttributeDependencies))
                .Concat(Inputs.SelectMany(i => i.PropertyAttributeDependencies))
                .Declarations()
                .SelectMany(t => t.Flatten())
                .Distinct()
                .Where(type => !Primitive.CanWrite(type) || type.IsEnum)
                .Where(type => type.IsGenericType
                    ? !includedTypes.Contains(type.GetGenericTypeDefinition())
                    : !includedTypes.Contains(type))
                .ToList();

            var count = -1;

            foreach (var type in missingTypes)
                Log.Debug("including: " + type.Namespace + "." + type.Name);

            while (missingTypes.Count != count)
            {
                count = missingTypes.Count;
                var dependencies = missingTypes.SelectMany(t => t.Dependencies())
                    .Concat(missingTypes.SelectMany(i => i.ClassAttributeDependencies()))
                    .Concat(missingTypes.SelectMany(i => i.PropertyAttributeDependencies()))
                    .Where(t => !missingTypes.Contains(t))
                    .Declarations()
                    .Where(type => !Primitive.CanWrite(type) || type.IsEnum)
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

            var attributes = Inputs.SelectMany(i => i.ClassAttributeDependencies)
                .Concat(Inputs.SelectMany(i => i.PropertyAttributeDependencies))
                .Distinct()
                .GroupBy(t => t.Assembly);

            var decoratorFiles = attributes.Select(g =>
            {
                var fileBuilder = new DecoratorFileBuilder { Assembly = g.Key };

                foreach (var attribute in g)
                    fileBuilder.Types.Add(attribute);

                fileBuilder.Build(this);
                return fileBuilder;
            });

            Inputs.AddRange(decoratorFiles);

            Console.WriteLine();
            foreach (var builder in Inputs)
            {
                builder.AddReferences(Inputs);


                File.WriteAllText(builder.File.Path, builder.File.ToString());
                Log.Info.FileWritten(System.IO.Path.GetFullPath(builder.File.Path));
            }

            Log.Info.JobComplete(_stopwatch.ElapsedMilliseconds);
        }

        public static void Main(string[] args)
        {
            Console.WriteLine();
            if (!args.Any()) throw new ArgumentException("Please specify the config file location");

            if (args.Length == 2)
            {
                if (args[1] == "-debug") Log.Level = LogLevel.Debug;
                else if (args[1] == "-info") Log.Level = LogLevel.Info;
                else if (args[1] == "-error") Log.Level = LogLevel.Error;
            }
            
            var configFileLocation = args.First();
            var context = Settings.Load(configFileLocation);

            foreach (var output in context.Outputs)
            {
                output.Transpile();
            }
        }
    }
}