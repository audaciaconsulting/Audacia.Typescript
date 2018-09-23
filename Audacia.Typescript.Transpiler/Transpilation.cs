using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Audacia.Typescript.Transpiler.Configuration;
using Audacia.Typescript.Transpiler.Documentation;
using Audacia.Typescript.Transpiler.Mappings;
using static System.Console;

namespace Audacia.Typescript.Transpiler
{
    /// <summary>Represents a single transpiler task and contains the main entry point for the program.</summary>
    public class Transpilation
    {
        private static readonly Stopwatch Stopwatch = new Stopwatch();
        private static Settings _settings;

        public static void Main(string[] args)
        {
            Stopwatch.Start();
            if (!args.Any()) throw new ArgumentException("Please specify the config file location");
            
            var configFileLocation = args.First();
            _settings = Settings.Load(configFileLocation);
            var documentation = XmlDocumentation.Load(_settings.Outputs
                .SelectMany(o => o.Inputs)
                .Select(i => i.Assembly));
            
            var outputs = _settings.Outputs
                .Select(setting => new FileMapping(setting, documentation))
                .ToArray();
            
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