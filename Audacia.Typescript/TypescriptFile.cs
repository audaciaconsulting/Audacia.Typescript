using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Audacia.Typescript
{
    public class TypescriptFile : IEnumerable<Element>
    {
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

        public override string ToString()
        {
            //var header = "THIS FILE WAS AUTO GENERATED ON " + DateTime.Now;
            var builder = new TypescriptBuilder();

            foreach (var element in Elements)
            {
                if (element == null) continue;
                element.Build(builder, null);

                if (element != Elements.LastOrDefault())
                    builder.AppendLine();
            }

            return builder.ToString();
        }

        public void Add(string statement)
        {
            Elements.Add(new Statement(statement));
        }
    }
}
