using System.Collections.Generic;

namespace CommonMarkSharp.Inlines
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
