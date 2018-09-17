using System;
using System.Collections.Generic;
using System.Linq;

namespace Audacia.Templating.Typescript.Build.Templates {
    public class InterfaceTemplate : Template
    {
        public InterfaceTemplate(Type type) : base(type) { }
        
        public override Element Build(IEnumerable<Template> context)
        {
            var  @interface = new Interface(Type.Name) { Modifiers = { Modifier.Export } };
			
            var interfaces = Type.GetInterfaces()
                .Where(i => context.Select(x => x.Type.Namespace).Contains(i.Namespace))
                .Select(x => x.Name);
			
            var members = Members(Type);
			
            foreach(var @base in interfaces)
                @interface.Extends.Add(@base);
			
            foreach(var member in members)
                @interface.Members.Add(member);
			
            Console.WriteLine($"Interface \"{@interface.Name}\" generated.");
            return @interface;
        }
    }
}