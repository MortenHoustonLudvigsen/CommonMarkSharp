using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Parsing.Inlines
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

        public override string ToString()
        {
            return string.Join("", Inlines.Select(c => c.ToString()));
        }
    }
}
