namespace CommonMarkSharp.Inlines
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
    }
}
