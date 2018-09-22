using System.Linq;

namespace Audacia.Typescript
{
    public class Indentation
    {
        public int Level { get; set; }
        
        public string Characters { get; }
        
        private Indentation(string characters)
        {
            Characters = characters;
        }
        
        public static Indentation Tabs(int count = 1) => new Indentation(new string('\t', count));
        
        public static Indentation Spaces(int count = 1) => new Indentation(new string(' ', count));

        public override string ToString() => string.Concat(Enumerable.Repeat(Characters, Level));
    }
}