using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Audacia.Typescript
{
    public class TypescriptFile : IEnumerable<Element>
    {
        public string Path { get; set; }
        
        public IList<Import> Imports { get; } = new List<Import>();
        
        public IList<Element> Elements { get; } = new List<Element>();

        public IEnumerable<Class> Classes => Elements.OfType<Class>();

        public IEnumerable<Function> Functions => Elements.OfType<Function>();

        public void Add(Element element) => Elements.Add(element);

        public void Add(IEnumerable<Element> elements)
        {
            foreach (var element in elements)
                Elements.Add(element);
        }

        public IEnumerator<Element> GetEnumerator() => Elements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(string statement) => Elements.Add(new Statement(statement));
        
        public override string ToString()
        {
            var builder = new TypescriptBuilder();

            builder.Append(new Comment("This file is generated from Audacia.Typescript.Transpiler. Any changes will be overwritten."), null)
                .NewLine()
                .NewLine();

            if (Imports.Any())
            {
                foreach (var import in Imports)
                    builder.Append(import, null).NewLine();

                builder.NewLine();
            }
            
            foreach (var element in Elements)
            {
                if (element == null) continue;

                element.Build(builder, null);

                if (element != Elements.LastOrDefault())
                    builder.NewLine().NewLine();
            }

            return builder.ToString();
        }
    }
}