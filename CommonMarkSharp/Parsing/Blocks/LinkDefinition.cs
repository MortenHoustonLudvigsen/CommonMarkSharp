using CommonMarkSharp.Parsing.Blocks;
using CommonMarkSharp.Parsing.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Parsing.Blocks
{
    public class LinkDefinition : LeafBlock
    {
        public LinkDefinition(Link link)
        {
            Link = link;
        }

        public Link Link { get; private set; }
        public string Id { get { return Link.Label.Literal; } }
    }
}
