using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Audacia.Typescript.Transpiler.Builders;
using Audacia.Typescript.Transpiler.Configuration;
using Audacia.Typescript.Transpiler.Documentation;
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
                .ToArray();

            var includedTypes = outputs.SelectMany(o => o.IncludedTypes);

            var missingTypes = outputs.SelectMany(o => o.Dependencies)
                .Where(type => type.IsGenericType
                    ? !includedTypes.Contains(type.GetGenericTypeDefinition())
                    : !includedTypes.Contains(type))
                .Where(type => !Primitive.CanWrite(type));


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