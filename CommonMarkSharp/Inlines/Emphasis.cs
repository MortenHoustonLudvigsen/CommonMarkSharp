using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Inlines
{
    public class Emphasis: InlineList
    {
        public Emphasis(params Inline[] inlines)
            : base(inlines)
        {
        }

        public Emphasis(IEnumerable<Inline> inlines)
            : base(inlines)
        {
        }
    }
}
