using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Audacia.Templating.Typescript.Build.Templates
{
    public class ClassTemplate : Template
    {
        private readonly IEnumerable<Type> _interfaces;
        private readonly IEnumerable<PropertyInfo> _properties;

        public override IEnumerable<Type> Dependencies => _properties
            .Select(p => p.PropertyType)
            .Concat(_interfaces)
            .Where(t => !t.Namespace.StartsWith(nameof(System)))
            .Where(t => t.Assembly != Type.Assembly);
        
        public ClassTemplate(Type type, IEnumerable<Settings> settings) : base(type, settings)
        {
            var namespaces = Settings.SelectMany(x => x.Namespaces);
            
            _interfaces = Type.GetInterfaces().Where(i => namespaces.Contains(i.Namespace));
            _properties = type.GetMembers(BindingFlags.Public | BindingFlags.Instance)
                .Where(mi => mi.MemberType == MemberTypes.Property)
                .Cast<PropertyInfo>();
        }

        public override Element Build(IEnumerable<Template> context)
        {
            var @class = new Class(Type.Name) { Modifiers = { Modifier.Export } };

            foreach(var @interface in _interfaces)
                @class.Implements.Add(@interface.Name);
			
            foreach(var property in _properties)
                @class.Members.Add(new Property(ToCamelCase(property.Name), GetTypeName(property.PropertyType)));
            
            Output(ConsoleColor.Green, "class", @class.Name);
            return @class;
        }
    }
}