namespace CommonMarkSharp.Inlines
{
    public class Image : Inline
    {
        public Image(Link link)
        {
            Link = link;
        }

        public Link Link { get; private set; }
    }
}
