using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Inlines
{
    public class LinkDestination: InlineList
    {
        public LinkDestination(string destination)
            : base(new InlineString(destination))
        {
        }

        public LinkDestination(params Inline[] inlines)
            : base(inlines)
        {
        }

        public LinkDestination(IEnumerable<Inline> inlines)
            : base(inlines)
        {
        }
    }
}
