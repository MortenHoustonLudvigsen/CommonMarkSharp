using System.Collections.Generic;

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
