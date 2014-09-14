using CommonMarkSharp.Inlines;

namespace CommonMarkSharp.Blocks
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
