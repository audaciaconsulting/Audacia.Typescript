using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Audacia.Typescript.Transpiler.Extensions;

namespace Audacia.Typescript.Transpiler.Mappings
{
    public class ClassMapping : Mapping
    {
        private readonly IEnumerable<Type> _interfaces;
        private readonly IEnumerable<Type> _typeArguments;
        private readonly IEnumerable<PropertyInfo> _properties;

        public override IEnumerable<Type> Dependencies => _properties
            .Select(p => p.PropertyType)
            .Concat(_interfaces)
            .Concat(_typeArguments)
            .Where(t => !t.Namespace.StartsWith(nameof(System)))
            .Where(t => t.Assembly != Type.Assembly);
        
        public ClassMapping(Type type, Settings settings) : base(type, settings)
        {
            _interfaces = Type.GetInterfaces().Where(i => !Settings.Namespaces.Any() || settings.Namespaces.Contains(i.Namespace));
            _typeArguments = type.GetGenericArguments();
            _properties = type.GetMembers(BindingFlags.Public | BindingFlags.Instance)
                .Where(mi => mi.MemberType == MemberTypes.Property)
                .Cast<PropertyInfo>();
        }

        public override Element Build()
        {
            var @class = new Class(Type.Name.Split('`').First()) { Modifiers = { Modifier.Export } };
            
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