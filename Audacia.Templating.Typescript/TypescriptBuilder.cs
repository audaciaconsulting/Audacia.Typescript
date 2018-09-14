using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Audacia.Templating.Typescript
{
    public class TypescriptBuilder
    {
        private readonly StringBuilder _builder = new StringBuilder();

        public int Indentation { get; set; }
        
        public TypescriptBuilder If(bool value, Action<TypescriptBuilder> action)
        {
            if (value) action(this);
            return this;
        }

        public TypescriptBuilder Join(IEnumerable<IElement> elements, IElement parent, string delimiter)
        {
            var d = string.Empty;
            foreach (var element in elements)
            {
                _builder.Append(d);
                element.Build(this, parent);
                d = delimiter;
            }

            return this;
        }

        public TypescriptBuilder Join(IEnumerable<string> strings, string delimiter)
        {
            var d = string.Empty;
            foreach (var element in strings)
            {
                Append(d).Append(element);
                d = delimiter;
            }

            return this;
        }

        public TypescriptBuilder Append(string s)
        {
            _builder.Append(s);
            return this;
        }

        public TypescriptBuilder Join(IList<string> strings, char delimiter)
        {
            for (var i = 0; i < strings.Count; i++)
            {
                {
                    if (i != 0) Append(delimiter);
                    Append(strings[i]);
                }
            }

            return this;
        }

        public TypescriptBuilder JoinDistinct(IEnumerable<string> strings, string delimiter)
        {
            var d = string.Empty;
            foreach (var element in strings.Distinct())
            {
                Append(d).Append(element);
                d = delimiter;
            }

            return this;
        }

        public TypescriptBuilder JoinDistinct(IEnumerable<string> strings, char delimiter)
        {
            var distinct = strings.Distinct().ToList();
            for (var i = 0; i < distinct.Count; i++)
            {
                if (i != 0) Append(delimiter);
                Append(distinct[i]);
            }

            return this;
        }

        public TypescriptBuilder Append(IElement element, Element parent)
        {
            return element.Build(this, parent);
        }

        public TypescriptBuilder Join<T>(IEnumerable<IModifier<T>> elements, string delimiter) where T : Element
        {
            var d = string.Empty;
            foreach (var element in elements)
            {
                Append(d).Append(element.Name);
                d = delimiter;
            }

            return this;
        }

        public TypescriptBuilder Join<T>(IList<IModifier<T>> elements, char delimiter) where T : Element
        {
            for (var i = 0; i < elements.Count; i++)
            {
                if (i != 0) Append(delimiter);
                Append(elements[i].Name);
            }

            return this;
        }

        public TypescriptBuilder Join<T>(IEnumerable<IMemberOf<T>> elements, Element parent, string delimiter) where T : Element
        {
            var d = string.Empty;
            foreach (var element in elements)
            {
                Append(d).Append(element, parent);
                d = delimiter;
            }

            return this;
        }

        public TypescriptBuilder Join<T>(IList<IMemberOf<T>> elements, Element parent, char delimiter) where T : Element
        {
            for (var i = 0; i < elements.Count; i++)
            {
                if (i != 0) Append(delimiter);
                Append(elements[i], parent);
            }

            return this;
        }

        public TypescriptBuilder Append(char c)
        {
            _builder.Append(c);
            return this;
        }

        public TypescriptBuilder Join(IList<Element> elements, Element parent, char delimiter)
        {
            for (var i = 0; i < elements.Count - 1; i++)
            {
                if (i != 0) Append(delimiter);
                elements[i].Build(this, parent);
            }

            return this;
        }

        public TypescriptBuilder AppendLine()
        {
            _builder.AppendLine();
            return this;
        }

        public TypescriptBuilder AppendLine(string line)
        {
            _builder.AppendLine(line);
            return this;
        }
        
        public TypescriptBuilder AppendLine(char c)
        {
            _builder.Append(c);
            _builder.AppendLine();
            return this;
        }
        
        public TypescriptBuilder AppendIndentation()
        {
            for (var i = 0; i < Indentation; i++)
                _builder.Append(' ').Append(' ').Append(' ').Append(' ');

            return this;
        }

        public TypescriptBuilder Indent()
        {
            Indentation++;
            return this;
        }
        public TypescriptBuilder Unindent()
        {
            if (Indentation != 0) Indentation--;
            return this;
        }

        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}