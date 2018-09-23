using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Audacia.Typescript.Transpiler.Configuration;
using Audacia.Typescript.Transpiler.Extensions;

namespace Audacia.Typescript.Transpiler.Mappings
{
    public class ClassMapping : TypeMapping
    {
        private readonly Type _baseType;
        private readonly IEnumerable<Type> _interfaces;
        private readonly IEnumerable<Type> _typeArguments;
        private readonly IEnumerable<PropertyInfo> _properties;
        
        public override IEnumerable<Type> Dependencies => _properties
            .Select(p => p.PropertyType)
            .Concat(new[] { _baseType }.Where(x => x != null))
            .Concat(_interfaces)
            .Concat(_typeArguments)
            .Where(t => !t.Namespace.StartsWith(nameof(System)));
        
        public ClassMapping(Type type, InputSettings settings) : base(type, settings)
        {
            _baseType = type.BaseType != typeof(object) ? type.BaseType : null;
            _typeArguments = type.GetGenericArguments();
            _interfaces = Type.GetDeclaredInterfaces()
                .Where(t => !t.Namespace.StartsWith(nameof(System)))
                .Where(i => Settings.Namespaces == null
                    || settings.Namespaces.Select(n => n.Name).Contains(i.Namespace));
            _properties = type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(mi => mi.MemberType == MemberTypes.Property)
                .Cast<PropertyInfo>();
        }

        public override Element Build()
        {
            var @class = new Class(Type.Name.Split('`').First())
            {
                Modifiers = { Modifier.Export },
                Extends = _baseType?.TypescriptName()
            };
            
            foreach(var @interface in _interfaces)
                @class.Implements.Add(@interface.TypescriptName());

            foreach (var typeArgument in _typeArguments)
                @class.TypeArguments.Add(typeArgument.TypescriptName());

            foreach(var property in _properties)
                @class.Members.Add(new Property(property.Name.CamelCase(), property.PropertyType.TypescriptName()));
            
            ReportProgress(ConsoleColor.Green, "class", @class.Name);
            return @class;
        }
    }
}