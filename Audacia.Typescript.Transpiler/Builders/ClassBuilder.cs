using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Audacia.Typescript.Transpiler.Configuration;
using Audacia.Typescript.Transpiler.Documentation;
using Audacia.Typescript.Transpiler.Extensions;

namespace Audacia.Typescript.Transpiler.Builders
{
    public class ClassBuilder : TypeBuilder
    {
        private readonly IEnumerable<Type> _interfaces;
        private readonly IEnumerable<Type> _typeArguments;
        private readonly IEnumerable<PropertyInfo> _properties;

        public object Instance { get; }

        public ClassBuilder(Type sourceType, InputSettings settings, XmlDocumentation documentation)
            : base(sourceType, settings, documentation)
        {
            if (Settings.Properties.Initialize)
                Instance = CreateInstance();

            _typeArguments = sourceType.GetGenericArguments();
            _interfaces = SourceType.GetDeclaredInterfaces()
                .Where(t => !t.Namespace.StartsWith(nameof(System)))
                .Where(i => Settings.Namespaces == null
                            || settings.Namespaces.Select(n => n.Name).Contains(i.Namespace));
            _properties = sourceType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(p => !p.GetIndexParameters().Any());

            // .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
        }

        public override Element Build()
        {
            var @class = new Class(SourceType.Name.SanitizeTypeName()) {Modifiers = {Modifier.Export}};

            if (Inherits != null)
                @class.Extends = Inherits.TypescriptName();

            var classDocumentation = Documentation?.ForClass(SourceType);

            if (classDocumentation != null)
                @class.Comment = classDocumentation.Summary;

            if (SourceType.IsAbstract) @class.Modifiers.Add(Modifier.Abstract);

            foreach (var @interface in _interfaces)
                @class.Implements.Add(@interface.TypescriptName());

            foreach (var typeArgument in _typeArguments)
                @class.TypeArguments.Add(typeArgument.TypescriptName());

            foreach (var source in _properties)
            {
                var getMethod = source.GetMethod;
                var target = new Property(source.Name.CamelCase(), source.PropertyType.TypescriptName());

                if (getMethod.IsAbstract) target.Modifiers.Add(Modifier.Abstract);

                if (getMethod.IsFamily) target.Modifiers.Add(Modifier.Protected);
                else target.Modifiers.Add(Modifier.Public);

                var propertyDocumentation = Documentation?.ForMember(source);

                if (propertyDocumentation != null)
                    target.Comment = propertyDocumentation.Summary;

                if (Settings.Properties?.Initialize ?? false)
                {
                    SetDefaultValue(source, target);
                }

                @class.Members.Add(target);
            }

            var illegalProp = @class.Properties.SingleOrDefault(p => p.Name == "constructor");
            {
                if (illegalProp != null)
                {
                    const string prefix = "_";
                    var newName = prefix + illegalProp.Name;
                    while (@class.Properties.Any(p => p.Name == newName))
                        newName = prefix + newName;

                    illegalProp.Name = newName;
                }
            }

            WriteLine(ConsoleColor.Green, "class", @class.Name);
            return @class;
        }

        private void SetDefaultValue(PropertyInfo source, Property target)
        {
            if (Instance != null)
            {
                object value = null;
                try { value = source.GetValue(Instance); }
                catch (TargetInvocationException e) when (e.InnerException != null)
                {
                    var exception = e.InnerException.GetType().Name;
                    var nameSpace = SourceType.Namespace;
                    var className = SourceType.Name;
                    var propertyName = source.Name;

                    //Console.WriteLine();
                    WriteLine(ConsoleColor.Red, "warn:", string.Empty);
                    Write(ConsoleColor.Blue, exception);
                    Write(" encountered inspecting ");
                    Write(nameSpace);
                    Write(".");
                    Write(ConsoleColor.Blue, className);
                    Write(".");
                    Write(propertyName);
                    WriteLine(ConsoleColor.Red, e.InnerException.Message, string.Empty);
                    Console.WriteLine();
                    Write(e.InnerException.StackTrace);
                    //Console.WriteLine();
                }

                if (value == null)
                {
                    target.Value = "null";
                    return;
                }

                if (source.PropertyType.IsEnum)
                {
                    target.Value = source.PropertyType
                        .TypescriptName() + "." + System.Enum
                        .GetName(source.PropertyType, value)
                        .CamelCase();

                    return;
                }

                var literal = Primitive.Literal(value);
                target.Value = literal ?? ("new " + value.GetType().TypescriptName() + "()");
            }

            if (Primitive.Array.CanWriteValue(source.PropertyType))
            {
                target.Value = Primitive.Literal(new object[0]);
                return;
            }

            if (Primitive.CanWrite(source.PropertyType) && source.PropertyType.IsPrimitive)
            {
                var @default = Activator.CreateInstance(source.PropertyType);
                target.Value = Primitive.Literal(@default);
            }

            else target.Value = "null";


        }

        private object CreateInstance()
        {
            if (SourceType.IsAbstract) return null;

            try
            {
                return Activator.CreateInstance(SourceType, true);
            }
            catch (MissingMethodException)
            {
                return null;
            }
            catch (TargetInvocationException e) when (e.InnerException != null)
            {
                var exception = e.InnerException.GetType().Name;
                var nameSpace = SourceType.Namespace;
                var className = SourceType.Name;

                WriteLine(ConsoleColor.Red, "warn:", string.Empty);
                Write(ConsoleColor.Blue, exception);
                Write(" encountered instantiating ");
                Write(nameSpace);
                Write(".");
                Write(ConsoleColor.Blue, className);
                Console.WriteLine();
                WriteLine(ConsoleColor.Red, "exception:", e.InnerException.Message);
                Console.WriteLine();
                Write(ConsoleColor.Red, e.InnerException.StackTrace);
                Console.WriteLine();

                return null;
            }
        }
    }
}