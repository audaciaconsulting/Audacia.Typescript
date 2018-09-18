using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using Audacia.Templating.Typescript.Build.Templates;

namespace Audacia.Templating.Typescript.Build
{
    public class Program
    {
        private static IDictionary<string, Settings> Settings { get; } = Build.Settings.Load();
        private static IDictionary<string, OutputFile> Outputs { get; } = new Dictionary<string, OutputFile>();

        public static void Main()
        {
            var rn = Environment.NewLine;
            
            var files = Settings.Values
                .Select(s => s.Output)
                .Distinct()
                .Select(path => new OutputFile(path));

            foreach (var file in files)
                Outputs.Add(file.Path, file);

            foreach (var setting in Settings)
            {
                var assembly = Assembly.LoadFrom(setting.Key);
                var templates = BuildTemplates(assembly, setting.Value).ToArray();
                
                foreach (var template in templates)
                    Outputs[setting.Value.Output].Templates.Add(template);
            }

            foreach (var file in Outputs)
            {
                // Write dependencies at the top of the file
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
                Console.WriteLine($"Typescript file \"{Path.GetFullPath(file.Value.Path)}\" written.");
                Console.ForegroundColor = ConsoleColor.White;
            }
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