using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Audacia.Typescript.Transpiler.Configuration;
using Audacia.Typescript.Transpiler.Extensions;

namespace Audacia.Typescript.Transpiler.Mappings 
{
    public class InterfaceMapping : TypeMapping
    {
        private readonly Type _baseType;
        private readonly IEnumerable<Type> _interfaces;
        private readonly IEnumerable<Type> _typeArguments;
        private readonly IEnumerable<PropertyInfo> _properties;

        public override IEnumerable<Type> Dependencies => _properties
            .Select(p => p.PropertyType)
            .Concat(_interfaces)
            .Where(t => !t.Namespace.StartsWith(nameof(System)))
            .Where(t => t.Assembly != Type.Assembly);
        
        public InterfaceMapping(Type type, InputSettings settings) : base(type, settings)
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
            var  @interface = new Interface(Type.Name.Split('`').First())
            {
                Modifiers = { Modifier.Export },
                Extends = { _baseType?.TypescriptName() }
            };
			
            foreach(var @base in _interfaces)
                @interface.Extends.Add(@base.TypescriptName());
			
            foreach (var typeArgument in _typeArguments)
                @interface.TypeArguments.Add(typeArgument.TypescriptName());
            
            foreach(var member in _properties)
                @interface.Members.Add(new Property(member.Name.CamelCase(), member.PropertyType.TypescriptName()));
			
            ReportProgress(ConsoleColor.Magenta, "interface", @interface.Name);
            return @interface;
        }
    }
}