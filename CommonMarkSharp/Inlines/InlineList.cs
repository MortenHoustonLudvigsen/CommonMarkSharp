using System.Collections.Generic;
using System.Linq;

namespace CommonMarkSharp.Inlines
{
    public class InlineList : Inline
    {
        public InlineList(params Inline[] inlines)
            : this(inlines.AsEnumerable())
        {
        }

        public InlineList(IEnumerable<Inline> inlines)
        {
            Inlines = inlines.ToList();
        }

        public IEnumerable<Inline> Inlines { get; private set; }
    }
}
