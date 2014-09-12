using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Parsing.Inlines
{
    public class LinkTitle : InlineList
    {
        public LinkTitle(string title)
            : base(new InlineString(title))
        {
        }

        public LinkTitle(params Inline[] inlines)
            : base(inlines)
        {
        }

        public LinkTitle(IEnumerable<Inline> inlines)
            : base(inlines)
        {
        }
    }
}
