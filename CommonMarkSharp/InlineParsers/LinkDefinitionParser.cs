using CommonMarkSharp.Inlines;

namespace CommonMarkSharp.InlineParsers
{
    public class LinkDefinitionParser : IInlineParser<Link>
    {
        public LinkDefinitionParser(Parsers parsers)
        {
            Parsers = parsers;
        }

        public Parsers Parsers { get; private set; }

        public string StartsWithChars { get { return " ["; } }

        public bool CanParse(Subject subject)
        {
            return subject.Char == ' ' || subject.Char == '[';
        }

        public Link Parse(ParserContext context, Subject subject)
        {
            if (!CanParse(subject)) return null;

            var saved = subject.Save();

            subject.AdvanceWhile(c => c == ' ', 3);

            if (subject.Char != '[')
            {
                return null;
            }

            var label = Parsers.LinkLabelParser.Parse(context, subject);
            if (label != null && subject.Char == ':')
            {
                subject.Advance();

                subject.SkipWhiteSpace();

                var destination = Parsers.LinkDestinationParser.Parse(context, subject);

                if (destination == null)
                {
                    saved.Restore();
                    return null;
                }
                var advanced = subject.SkipWhiteSpace();

                var title = new LinkTitle();
                if (destination == null || advanced > 0)
                {
                    title = Parsers.LinkTitleParser.Parse(context, subject);
                    subject.SkipWhiteSpace();
                }

                if (subject.EndOfString || subject[-1] == '\n')
                {
                    return new Link(label, destination, title);
                }
            }

            saved.Restore();
            return null;
        }
    }
}
