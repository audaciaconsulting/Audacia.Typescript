using System.Collections.Generic;

namespace Audacia.Templating.Typescript.Collections
{
    public class ArgumentList : List<Argument>
    {
        public void Add(string name)
        {
            Add(new Argument(name));
        }

        public void Add(string name, string type)
        {
            Add(new Argument(name, type));
        }
    }
}
