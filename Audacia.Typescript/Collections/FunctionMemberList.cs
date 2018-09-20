using System.Collections.Generic;

namespace Audacia.Typescript.Collections
{
    public class FunctionMemberList : List<IMemberOf<Function>>
    {
        public void Add(string statement)
        {
            Add(new Statement(statement));
        }
    }
}