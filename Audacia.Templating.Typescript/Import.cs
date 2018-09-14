using System.Collections.Generic;

namespace Audacia.Templating.Typescript
{
    public class Import
    {
        public string FileName { get; set; }

        public IList<string> Types { get; set; } = new List<string>();
    }
}