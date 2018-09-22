using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Audacia.Typescript.Transpiler.Builders;

namespace Audacia.Typescript.Transpiler
{
    public class Program
    {
        private static readonly Stopwatch Stopwatch = new Stopwatch();
        private static Settings Settings { get; set; } 
        private static IDictionary<string, OutputFile> Outputs { get; } = new Dictionary<string, OutputFile>();

        public static void Main(string[] args)
        {
            Stopwatch.Start();
            if (!args.Any()) throw new ArgumentException("Please specify the config file location");
            
            var configFileLocation = args.First();
            Settings = Settings.Load(configFileLocation);
            
            var rn = Environment.NewLine;
            
            var files = Settings.Assemblies
                .SelectMany(s => s.Outputs)
                .Distinct()
                .Select(outputSettings => new OutputFile(outputSettings.Path));

            foreach (var file in files)
                Outputs.Add(file.Path, file);

            foreach (var setting in Settings.Assemblies)
            {
                var assembly = Assembly.LoadFrom(setting.Assembly);
                var builders = CreateBuilders(assembly, setting).ToArray();

                foreach (var builder in builders)
                {
                    foreach(var output in setting.Outputs)
                        Outputs[output.Path].Builders.Add(builder);
                }
            }

            foreach (var file in Outputs)
            {
                // Write dependencies at the top of the file
                // TODO: This should be functionality in TypescriptFile.cs
                var dependencies = file.Value.Dependencies;

                var references = Outputs.Except(new[] {file})
                    .Where(o => o.Value.IncludedTypes.Any(t => dependencies.Contains(t)));

                var includes = string.Empty;
                foreach (var reference in references)
                {
                    var source = new Uri(Path.GetFullPath(file.Value.Path));
                    var target = new Uri(Path.GetFullPath(reference.Value.Path));
                    var relativePath = source.MakeRelativeUri(target)
                        .ToString()
                        .Replace(".ts", string.Empty);

                    var types = file.Value.Dependencies
                        .Where(d => reference.Value.IncludedTypes.Contains(d))
                        .Select(d => new string(' ', 4) + d.Name)
                        .Distinct();

                    includes += $"import {{ {rn}{string.Join(',' + rn, types)}{rn} }} from \"./{relativePath}\"{rn}";
                }
                
                var content = file.Value.Build(Outputs);
                File.WriteAllText(file.Key, includes + rn + content);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine();
                Console.WriteLine($"Typescript file \"{Path.GetFullPath(file.Value.Path)}\" written.");
                Console.WriteLine();
                Console.ResetColor();
            }
            
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Typescript transpile completed in {Stopwatch.ElapsedMilliseconds}ms.");
            Console.ResetColor();
            
        }

        private static IEnumerable<Builder> CreateBuilders(Assembly assembly, AssemblySettings settings)
        {
            var types = assembly.GetTypes()
                .Where(t => settings.Namespaces.Any(n => n.Name == t.Namespace))
                .Where(t => t.IsClass || t.IsInterface || t.IsEnum)
                .Where(t => t.IsPublic && !t.IsNested);

            foreach (var type in types)
            {
                if (type.IsClass) yield return new ClassBuilder(type, Settings);
                else if (type.IsInterface) yield return new InterfaceBuilder(type, Settings);
                else if (type.IsEnum) yield return new EnumBuilder(type, Settings);
            }
        }
    }
}