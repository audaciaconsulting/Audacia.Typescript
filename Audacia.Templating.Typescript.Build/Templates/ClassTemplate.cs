using System;
using System.Collections.Generic;
using System.Linq;

namespace Audacia.Templating.Typescript.Build.Templates
{
    public class ClassTemplate : Template
    {
        public ClassTemplate(Type type) : base(type) { }
        
        public override Element Build(IEnumerable<Template> context)
        {
            var @class = new Class(Type.Name) { Modifiers = { Modifier.Export } };

            var interfaces = Type.GetInterfaces()
                .Where(i => context.Select(x => x.Type.Namespace).Contains(i.Namespace))
                .Select(x => x.Name);
			
            var members = Members(Type);
			
            foreach(var @interface in interfaces)
                @class.Implements.Add(@interface);
			
            foreach(var member in members)
                @class.Members.Add(member);
			
            Console.WriteLine($"Class \"{@class.Name}\" generated.");
            return @class;
        }
    }
}