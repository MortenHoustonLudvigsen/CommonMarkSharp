using System.Collections.Generic;

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
