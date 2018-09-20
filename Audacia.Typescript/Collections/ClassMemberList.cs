using System.Collections.Generic;

namespace Audacia.Typescript.Collections
{
    public class ClassMemberList : List<IMemberOf<Class>>
    {
        public void Add(string propertyName, string propertyType)
        {
            Add(new Property(propertyName, propertyType));
        }
    }
}