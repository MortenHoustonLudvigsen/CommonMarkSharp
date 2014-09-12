using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Parsing.Inlines
{
    public class StrongEmphasis : Emphasis
    {
        public StrongEmphasis(params Inline[] inlines)
            : base(inlines)
        {
        }

        public StrongEmphasis(IEnumerable<Inline> inlines)
            : base(inlines)
        {
        }
    }
}
