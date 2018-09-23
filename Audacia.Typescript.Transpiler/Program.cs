using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Audacia.Typescript.Transpiler.Builders;
using static System.Console;

namespace Audacia.Typescript.Transpiler
{
    public class Program
    {
        private static readonly Stopwatch Stopwatch = new Stopwatch();
        private static Settings Settings { get; set; } 

        public static void Main(string[] args)
        {
            Stopwatch.Start();
            if (!args.Any()) throw new ArgumentException("Please specify the config file location");
            
            var configFileLocation = args.First();
            Settings = Settings.Load(configFileLocation);
            
            var files = new Dictionary<TypescriptFile, IEnumerable<Builder>>();
            foreach (var settings in Settings.Outputs)
            {
                var file = new TypescriptFile { Path = settings.Path };
                
                files[file] = settings.Inputs
                    .Distinct()
                    .Select(input => Assembly.LoadFrom(input.Assembly))
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(t => Settings.Namespaces == null || Settings.Namespaces.Any(n => n == t.Namespace))
                    .Where(t => t.IsClass || t.IsInterface || t.IsEnum)
                    .Where(t => t.IsPublic && !t.IsNested)
                    .Select(x => Builder.Create(x, Settings));

                foreach (var builder in files[file])
                    file.Elements.Add(builder.Build());
            }

            foreach (var file in files)
            {
                var dependencies = file.Value
                    .SelectMany(x => x.Dependencies)
                    .Distinct();
                
                var references = files.Except(new[] {file})
                    .Where(o => o.Value.Any(t => dependencies.Contains(t.Type)));

                foreach (var reference in references)
                {
                    var source = new Uri(Path.GetFullPath(file.Key.Path));
                    var target = new Uri(Path.GetFullPath(reference.Key.Path));
                    var relativePath = source.MakeRelativeUri(target)
                        .ToString();

                    if (relativePath.EndsWith(".ts"))
                        relativePath = relativePath.Substring(0, relativePath.Length - 3);

                    var types = file.Value.SelectMany(x => x.Dependencies)
                        .Where(d => reference.Value.Select(x => x.Type).Contains(d))
                        .Distinct()
                        .Select(d => d.Name);

                    file.Key.Imports.Add(new Import(relativePath, types));
                }
                
                File.WriteAllText(file.Key.Path, file.Key.ToString());

                ForegroundColor = ConsoleColor.Green;
                WriteLine();
                WriteLine($"Typescript file \"{Path.GetFullPath(file.Key.Path)}\" written.");
                WriteLine();
                ResetColor();
            }
            
            ForegroundColor = ConsoleColor.Green;
            WriteLine($"Typescript transpile completed in {Stopwatch.ElapsedMilliseconds}ms.");
            ResetColor();
            
        }
    }
}