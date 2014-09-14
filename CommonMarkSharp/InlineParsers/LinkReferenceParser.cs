using CommonMarkSharp.Inlines;

namespace CommonMarkSharp.InlineParsers
{
    public class LinkReferenceParser : IInlineParser<LinkReference>
    {
        public LinkReferenceParser(Parsers parsers)
        {
            Parsers = parsers;
        }

        public Parsers Parsers { get; private set; }

        public string StartsWithChars { get { return "["; } }

        public bool CanParse(Subject subject)
        {
            return subject.Char == '[';
        }

        public LinkReference Parse(ParserContext context, Subject subject)
        {
            if (!CanParse(subject)) return null;

            var saved = subject.Save();

            var label = Parsers.LinkLabelParser.Parse(context, subject);
            if (label != null)
            {
                subject.SkipWhiteSpace();

                LinkLabel referenceLabel;

                if (subject.StartsWith("[]"))
                {
                    // This is a collapsed reference link
                    subject.Advance(2);
                    referenceLabel = label;
                }
                else
                {
                    // This is a full reference link or a shortcut reference link
                    referenceLabel = Parsers.LinkLabelParser.Parse(context, subject) ?? label;
                }
                var link = context.Document.FindLink(referenceLabel.Literal);
                if (link != null)
                {
                    return new LinkReference(label, link);
                }
            }

            saved.Restore();
            return null;
        }
    }
}
