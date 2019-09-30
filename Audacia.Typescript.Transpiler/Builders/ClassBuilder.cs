using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Audacia.Typescript.Transpiler.Extensions;
using Audacia.Typescript.Transpiler.Logging;

namespace Audacia.Typescript.Transpiler.Builders
{
    public class ClassBuilder : TypeBuilder
    {
        private readonly IEnumerable<Type> _interfaces;
        private readonly IEnumerable<Type> _typeArguments;
        private readonly IEnumerable<PropertyInfo> _properties;
        private readonly IList<Attribute> _attributes;

        public object Instance { get; }

        public ClassBuilder(Type sourceType, FileBuilder input, Transpilation outputContext)
            : base(sourceType, input, outputContext)
        {
            if (OutputContext.Properties.Initialize)
                Instance = CreateInstance();

            _typeArguments = sourceType.GetGenericArguments();
            _interfaces = SourceType.GetDeclaredInterfaces()
                .Where(t => t.Namespace == null || !t.Namespace.StartsWith(nameof(System)))
                .Where(i => !input.Namespaces.Any() || input.Namespaces.Select(n => n.Name).Contains(i.Namespace));
            _properties = sourceType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public |
                                                   BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(p => !p.GetIndexParameters().Any())
                .Where(p => !p.GetMethod.IsPrivate)
                .Where(p => !p.GetMethod.IsAssembly);
            _attributes = sourceType.GetCustomAttributes(false).Cast<Attribute>().Where(t => t.GetType().IsPublic).ToList();
        }

        public override Element Build()
        {
            var @class = new Class(SourceType.Name.SanitizeTypeName()) {Modifiers = {Modifier.Export}};
            Log.Info.Class(@class);

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

            if (!SourceType.IsAbstract)
            {
                foreach (var attribute in _attributes)
                {
                    var name = attribute.GetType().ClassDecoratorName();
                    var arguments = new ObjectLiteral(attribute);
                    @class.Decorators.Add(new Decorator(name, new[] {arguments}));
                }
            }

            foreach (var source in _properties)
            {
                var getMethod = source.GetMethod;
                var target = new Property(source.Name.CamelCase(), source.PropertyType.TypescriptName());
                var attributes = source.GetCustomAttributes(false).Where(t => t.GetType().IsPublic);

                if (getMethod.IsAbstract) target.Modifiers.Add(Modifier.Abstract);

                if (getMethod.IsFamily) target.Modifiers.Add(Modifier.Protected);
                else target.Modifiers.Add(Modifier.Public);

                var propertyDocumentation = Documentation?.ForMember(source);

                if (propertyDocumentation != null)
                    target.Comment = propertyDocumentation.Summary;

                if (OutputContext.Properties?.Initialize ?? false)
                {
                    SetDefaultValue(source, target);
                }

                foreach (var attribute in attributes)
                {
                    var name = attribute.GetType().PropertyDecoratorName();
                    var arguments = new ObjectLiteral(attribute);
                    target.Decorators.Add(new Decorator(name, new[] {arguments}));
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
            return @class;
        }

        private void SetDefaultValue(PropertyInfo source, Property target)
        {
            if (Instance != null)
            {
                object value = null;
                try
                {
                    value = source.GetValue(Instance);
                }
                catch (TargetInvocationException e) when (e.InnerException != null)
                {
                    var exception = e.InnerException;
                    Log.Warn.FailedToReadProperty(exception, source);
                }

                if (value == null)
                {
                    target.Value = "null";
                    return;
                }

                if (source.PropertyType.IsEnum && source.PropertyType.GetCustomAttribute<FlagsAttribute>() == null)
                {
                    var name = System.Enum.GetName(source.PropertyType, value);
                    
                    // Watch out for enums without a valid default value (i.e. no properties with value "0")
                    if (string.IsNullOrEmpty(name)) target.Value =  "null";
                    else target.Value = source.PropertyType
                                       .Name + "." + name
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
            if (SourceType.ContainsGenericParameters) return null;
            if (SourceType.GetConstructors().All(c => c.GetParameters().Length != 0)) return null;

            try
            {
                return Activator.CreateInstance(SourceType, true);
            }
            catch (MissingMethodException e)
            {
                Log.Warn.FailedToInstantiateType(e, SourceType);
                return null;
            }
            catch (NotSupportedException e)
            {
                Log.Warn.FailedToInstantiateType(e, SourceType);
                return null;
            }
            catch (Exception e)
            {
                if (e is TargetInvocationException x) e = x;
                Log.Warn.FailedToInstantiateType(e, SourceType);

                return null;
            }
        }
    }
}