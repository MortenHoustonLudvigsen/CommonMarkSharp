using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Parsing.Inlines
{
    public class Link : Inline
    {
        public Link(LinkLabel label, LinkDestination destination, LinkTitle title)
        {
            Label = label;
            Destination = destination ?? new LinkDestination();
            Title = title ?? new LinkTitle();
        }

        public LinkLabel Label { get; private set; }
        public LinkDestination Destination { get; private set; }
        public LinkTitle Title { get; private set; }

        public override string ToString()
        {
            return string.Format("[{0}]({1} {2})", Label, Destination, Title);
        }
    }
}
