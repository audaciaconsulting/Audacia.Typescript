using System.Collections;
using System.Collections.Generic;

namespace Audacia.Templating.Typescript
{
    public abstract class Accessor : Element, IEnumerable<IMemberOf<Function>>
    {
        public IList<IMemberOf<Function>> Statements { get; } = new List<IMemberOf<Function>>();

        public void Add(string code) => Statements.Add(new Statement(code));

        public void Add(IMemberOf<Function> member) => Statements.Add(member);

        public IEnumerator<IMemberOf<Function>> GetEnumerator() => Statements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}