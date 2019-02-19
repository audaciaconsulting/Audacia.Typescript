using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Audacia.Typescript.Transpiler.Builders;
using Audacia.Typescript.Transpiler.Configuration;
using Audacia.Typescript.Transpiler.Documentation;
using Audacia.Typescript.Transpiler.Extensions;
using static System.Console;

namespace Audacia.Typescript.Transpiler
{
    // TODO: Finish this

    /// <summary>Represents a single transpiler task and contains the main entry point for the program.</summary>
    public class Transpilation
    {
        public static Settings Settings { get; private set; }

        private static readonly Stopwatch Stopwatch = new Stopwatch();

        public static void Main(string[] args)
        {
            Stopwatch.Start();

            WriteLine();
            if (!args.Any()) throw new ArgumentException("Please specify the config file location");

            var configFileLocation = args.First();
            Settings = Settings.Load(configFileLocation);

            var assemblies = Settings.Outputs.SelectMany(o => o.Inputs).Select(i => i.Assembly);
            var documentation = XmlDocumentation.Load(assemblies);

            var outputs = Settings.Outputs
                .Select(setting => new FileBuilder(setting, documentation))
                .ToList();

            var includedTypes = outputs.SelectMany(o => o.IncludedTypes);

            var missingTypes = outputs.SelectMany(o => o.Dependencies)
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
                var name = string.Join(".", group.Key.GetName().Name
                    .TrimEnd()
                    .Split('.')
                    .Reverse()
                    .Skip(1) // remove file extension
                    .Reverse()
                    .Select(s => s.CamelCase()))
                    + ".ts";

                var path = Path.GetDirectoryName(outputs.First().Path) ?? string.Empty;
                var settings = new OutputSettings(Path.Combine(path, name));
                var output = new FileBuilder(settings, group);
                outputs.Add(output);
            }

            foreach (var file in outputs)
            {
                file.AddReferences(outputs);

                File.WriteAllText(file.Path, file.Build());

                ForegroundColor = ConsoleColor.Green;
                WriteLine();
                WriteLine($"Typescript file \"{Path.GetFullPath(file.Path)}\" written.");
                WriteLine();
                ResetColor();
            }


            ForegroundColor = ConsoleColor.Green;
            WriteLine($"Typescript transpile completed in {Stopwatch.ElapsedMilliseconds}ms.");
            ResetColor();
        }
    }
}