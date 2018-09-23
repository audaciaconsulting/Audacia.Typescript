using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Audacia.Typescript.Transpiler.Extensions;

namespace Audacia.Typescript.Transpiler.Mappings {
    public class InterfaceMapping : Mapping
    {
        private readonly IEnumerable<Type> _interfaces;
        private readonly IEnumerable<Type> _typeArguments;
        private readonly IEnumerable<PropertyInfo> _properties;

        public override IEnumerable<Type> Dependencies => _properties
            .Select(p => p.PropertyType)
            .Concat(_interfaces)
            .Where(t => !t.Namespace.StartsWith(nameof(System)))
            .Where(t => t.Assembly != Type.Assembly);
        
        public InterfaceMapping(Type type, Settings settings) : base(type, settings)
        {
            _interfaces = Type.GetInterfaces().Where(i => settings.Namespaces == null || settings.Namespaces.Contains(i.Namespace));
            _typeArguments = type.GetGenericArguments();
            _properties = type.GetMembers(BindingFlags.Public | BindingFlags.Instance)
                .Where(mi => mi.MemberType == MemberTypes.Property)
                .Cast<PropertyInfo>();
        }

        public override Element Build()
        {
            var  @interface = new Interface(Type.Name.Split('`').First()) { Modifiers = { Modifier.Export } };
			
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