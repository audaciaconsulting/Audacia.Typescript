using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Audacia.Typescript.Transpiler.Mappings;
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
            
            var files = new Dictionary<TypescriptFile, IEnumerable<Mapping>>();
            foreach (var settings in Settings.Outputs)
            {
                var file = new TypescriptFile { Path = settings.Path };
                
                files[file] = settings.Inputs
                    .Distinct()
                    .SelectMany(input =>
                    {
                        var types = Assembly.LoadFrom(input.Assembly)
                            .GetTypes()
                            .Where(t => input.Namespaces == null 
                                || input.Namespaces.Any(n => n.Name == t.Namespace));
                        
                        // Filter out subtypes of generics- we only want the top one in the inheritance hierarchy
                        return types.Where(type => !types
                            .Any(other => type.Namespace == other.Namespace
                                && type.Name.Split('`').First() == other.Name.Split('`').First()
                                && other.GetGenericArguments().Length > type.GetGenericArguments().Length));
                    })
                    .Where(t => t.IsClass || t.IsInterface || t.IsEnum)
                    .Where(t => t.IsPublic && !t.IsNested)
                    .Select(x => Mapping.Create(x, Settings));

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
                    var relativePath = "./" + source.MakeRelativeUri(target);

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
                ResetColor();
            }
            
            WriteLine();
            ForegroundColor = ConsoleColor.Green;
            WriteLine($"Typescript transpile completed in {Stopwatch.ElapsedMilliseconds}ms.");
            ResetColor();
            
        }
    }
}