namespace CommonMarkSharp.Inlines
{
    public class ImageReference : Inline
    {
        public ImageReference(LinkReference linkReference)
        {
            LinkReference = linkReference;
        }

        public LinkReference LinkReference { get; private set; }
    }
}
