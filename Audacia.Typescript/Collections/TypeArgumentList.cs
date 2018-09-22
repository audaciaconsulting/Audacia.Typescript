using System.Collections.Generic;

namespace Audacia.Typescript.Collections
{
    public class TypeArgumentList : List<TypeArgument>
    {
        public void Add(string name)
        {
            Add(new TypeArgument(name));
        }

        public void Add(string name, string extends)
        {
            Add(new TypeArgument(name, extends));
        }
    }
}