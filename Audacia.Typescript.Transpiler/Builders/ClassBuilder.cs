using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Audacia.Typescript.Transpiler.Builders
{
    public class ClassBuilder : Builder
    {
        private readonly IEnumerable<Type> _interfaces;
        private readonly IEnumerable<PropertyInfo> _properties;

        public override IEnumerable<Type> Dependencies => _properties
            .Select(p => p.PropertyType)
            .Concat(_interfaces)
            .Where(t => !t.Namespace.StartsWith(nameof(System)))
            .Where(t => t.Assembly != Type.Assembly);
        
        public ClassBuilder(Type type, Settings settings) : base(type, settings)
        {
            _interfaces = Type.GetInterfaces().Where(i => Settings.Namespaces == null || settings.Namespaces.Contains(i.Namespace));
            _properties = type.GetMembers(BindingFlags.Public | BindingFlags.Instance)
                .Where(mi => mi.MemberType == MemberTypes.Property)
                .Cast<PropertyInfo>();
        }

        public override Element Build()
        {
            var @class = new Class(Type.Name) { Modifiers = { Modifier.Export } };

            foreach(var @interface in _interfaces)
                @class.Implements.Add(@interface.Name);
			
            foreach(var property in _properties)
                @class.Members.Add(new Property(ToCamelCase(property.Name), GetTypeName(property.PropertyType)));
            
            ReportProgress(ConsoleColor.Green, "class", @class.Name);
            return @class;
        }
    }
}