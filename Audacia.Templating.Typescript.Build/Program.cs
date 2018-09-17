using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Audacia.Templating.Typescript.Build.Templates;

namespace Audacia.Templating.Typescript.Build
{
    public class Program
    {
        private static IDictionary<string, Settings> Settings { get; } = Build.Settings.Load();
        private static IDictionary<string, OutputFile> Files { get; } = new Dictionary<string, OutputFile>();

        public static void Main()
        {
            var files = Settings.Values
                .SelectMany(s => s.Output)
                .Distinct()
                .Select(path => new OutputFile(path));

            foreach (var file in files)
                Files.Add(file.Path, file);

            foreach (var setting in Settings)
            {
                var assembly = Assembly.LoadFrom(setting.Key);
                var templates = BuildTemplates(assembly, setting.Value).ToArray();
                
                foreach (var output in setting.Value.Output)
                {
                    foreach(var template in templates)
                        Files[output].Templates.Add(template);
                }                
            }
            
            foreach (var file in Files)
            {
                File.WriteAllText(file.Key, file.Value.Build(Files));

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Typescript file \"{Path.GetFullPath(file.Value.Path)}\" written.");
                Console.ForegroundColor = ConsoleColor.White;
            }
                
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Transformation complete.");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static IEnumerable<Template> BuildTemplates(Assembly assembly, Settings settings)
        {
            var types = assembly.GetTypes()
                .Where(t => settings.Namespaces.Contains(t.Namespace))
                .Where(t => t.IsClass || t.IsInterface || t.IsEnum)
                .Where(t => t.IsPublic && !t.IsNested);

            foreach (var type in types)
            {
                var assemblySettings = Settings.Select(s => s.Value);
                
                if (type.IsClass) yield return new ClassTemplate(type, assemblySettings);
                else if (type.IsInterface) yield return new InterfaceTemplate(type, assemblySettings);
                else if (type.IsEnum) yield return new EnumTemplate(type, assemblySettings);
            }
        }
    }
}