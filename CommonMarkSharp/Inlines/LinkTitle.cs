using System.Collections.Generic;

namespace CommonMarkSharp.Inlines
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
