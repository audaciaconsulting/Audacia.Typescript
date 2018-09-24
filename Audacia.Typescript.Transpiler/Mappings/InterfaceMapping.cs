using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Audacia.Typescript.Transpiler.Configuration;
using Audacia.Typescript.Transpiler.Documentation;
using Audacia.Typescript.Transpiler.Extensions;

namespace Audacia.Typescript.Transpiler.Mappings 
{
    // TODO: Consolidate with Class.cs, its basically the same
    public class InterfaceMapping : TypeMapping
    {
        private readonly IEnumerable<Type> _interfaces;
        private readonly IEnumerable<Type> _typeArguments;
        private readonly IEnumerable<PropertyInfo> _properties;

        public override IEnumerable<Type> Dependencies => _properties
            .Select(p => p.PropertyType)
            .Concat(_interfaces)
            .Concat(_typeArguments)
            .Where(t => !t.Namespace.StartsWith(nameof(System)));
        
        public InterfaceMapping(Type sourceType, InputSettings settings, XmlDocumentation documentation) 
            : base(sourceType, settings, documentation)
        {
            _typeArguments = sourceType.GetGenericArguments();
            _interfaces = SourceType.GetDeclaredInterfaces()
                .Where(t => !t.Namespace.StartsWith(nameof(System)))
                .Where(i => Settings.Namespaces == null
                            || settings.Namespaces.Select(n => n.Name).Contains(i.Namespace));
            _properties = sourceType.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(mi => mi.MemberType == MemberTypes.Property)
                .Cast<PropertyInfo>();
        }

        public override Element Build()
        {
            var  @interface = new Interface(SourceType.Name.Split('`').First()) { Modifiers = { Modifier.Export } };
            
            foreach(var @base in _interfaces)
                @interface.Extends.Add(@base.TypescriptName());
			
            var classDocumentation = Documentation.ForClass(SourceType);
            if (classDocumentation != null)
                @interface.Comment = classDocumentation.Summary;
            
            foreach (var typeArgument in _typeArguments)
                @interface.TypeArguments.Add(typeArgument.TypescriptName());

            foreach (var member in _properties)
            {
                var property = new Property(member.Name.CamelCase(), member.PropertyType.TypescriptName());
                var propertyDocumentation = Documentation.ForMember(member);
                
                if (propertyDocumentation != null)
                    property.Comment = propertyDocumentation.Summary;
                
                @interface.Members.Add(property);
            }
			
            ReportProgress(ConsoleColor.Magenta, "interface", @interface.Name);
            return @interface;
        }
    }
}