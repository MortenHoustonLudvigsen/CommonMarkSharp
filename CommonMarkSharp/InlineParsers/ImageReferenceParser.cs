using CommonMarkSharp.Inlines;

namespace CommonMarkSharp.InlineParsers
{
    public class ImageReferenceParser : IInlineParser<ImageReference>
    {
        public ImageReferenceParser(Parsers parsers)
        {
            Parsers = parsers;
        }

        public Parsers Parsers { get; private set; }

        public string StartsWithChars { get { return "!"; } }

        public bool CanParse(Subject subject)
        {
            return subject.Char == '!';
        }

        public ImageReference Parse(ParserContext context, Subject subject)
        {
            if (!CanParse(subject)) return null;

            var saved = subject.Save();
            subject.Advance();

            var reference = Parsers.LinkReferenceParser.Parse(context, subject);
            if (reference != null)
            {
                return new ImageReference(reference);
            }

            saved.Restore();
            return null;
        }
    }
}
