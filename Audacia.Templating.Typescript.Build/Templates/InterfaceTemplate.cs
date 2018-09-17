using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Audacia.Templating.Typescript.Build.Templates {
    public class InterfaceTemplate : Template
    {
        private readonly IEnumerable<Type> _interfaces;
        private readonly IEnumerable<PropertyInfo> _properties;

        public override IEnumerable<Type> Dependencies => _properties
            .Select(p => p.PropertyType)
            .Concat(_interfaces)
            .Where(t => !t.Namespace.StartsWith(nameof(System)))
            .Where(t => t.Assembly != Type.Assembly);
        
        public InterfaceTemplate(Type type, IEnumerable<Settings> settings) : base(type, settings)
        {
            var namespaces = Settings.SelectMany(x => x.Namespaces);
            
            _interfaces = Type.GetInterfaces().Where(i => namespaces.Contains(i.Namespace));
            _properties = type.GetMembers(BindingFlags.Public | BindingFlags.Instance)
                .Where(mi => mi.MemberType == MemberTypes.Property)
                .Cast<PropertyInfo>();
        }

        public override Element Build(IEnumerable<Template> context)
        {
            var  @interface = new Interface(Type.Name) { Modifiers = { Modifier.Export } };
			
            foreach(var @base in _interfaces)
                @interface.Extends.Add(@base.Name);
			
            foreach(var member in _properties)
                @interface.Members.Add(new Property(ToCamelCase(member.Name), GetTypeName(member.PropertyType)));
			
            ReportProgress(ConsoleColor.Magenta, "interface", @interface.Name);
            return @interface;
        }
    }
}