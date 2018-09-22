using System.Collections.Generic;
using System.Text;

namespace Audacia.Typescript
{
    public class TypescriptBuilder
    {
        public Indentation Indentation { get; } = Indentation.Spaces(4);
        
        public Line CurrentLine { get; } = new Line();
        
        private readonly StringBuilder _builder = new StringBuilder();
    
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

        public TypescriptBuilder Append(IElement element, Element parent)
        {
            if(CurrentLine.Blank) _builder.Append(Indentation);
            
            CurrentLine.Blank = false;
            return element.Build(this, parent);    
        }
        
        public TypescriptBuilder Append(string s)
        {
            if(CurrentLine.Blank) _builder.Append(Indentation);
            
            CurrentLine.Blank = false;
            _builder.Append(s);            
            return this;
        }
        
        public TypescriptBuilder Append(char c)
        {
            if(CurrentLine.Blank) _builder.Append(Indentation);
            CurrentLine.Blank = false;
            
            _builder.Append(c);
            return this;
        }
        
        public TypescriptBuilder NewLine()
        {
            _builder.AppendLine();
            CurrentLine.Blank = true;
            return this;
        }
        
        public TypescriptBuilder Indent()
        {
            Indentation.Level++;
            return this;
        }
        
        public TypescriptBuilder Unindent()
        {
            if (Indentation.Level != 0) Indentation.Level--;
            return this;
        }

        public override string ToString()
        {
            return _builder.ToString();
        }
        
        public class Line { public bool Blank { get; set; } }
    }
}