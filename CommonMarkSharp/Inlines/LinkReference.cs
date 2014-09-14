namespace CommonMarkSharp.Inlines
{
    public class LinkReference : Inline
    {
        public LinkReference(LinkLabel label, Link link)
        {
            Label = label;
            Link = link;
        }

        public LinkLabel Label { get; set; }
        public Link Link { get; private set; }
    }
}
