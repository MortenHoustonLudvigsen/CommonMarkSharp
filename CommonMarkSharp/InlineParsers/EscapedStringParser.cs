using CommonMarkSharp.Inlines;
using System;

namespace CommonMarkSharp.InlineParsers
{
    public class EscapedStringParser : IInlineParser<InlineString>
    {
        public EscapedStringParser(Parsers parsers)
        {
            Parsers = parsers;
            OthersParser = new Lazy<IInlineParser<InlineString>>(() => new AllExceptParser(parsers.EscapedCharParser.StartsWithChars));
        }

        public Parsers Parsers { get; private set; }
        public Lazy<IInlineParser<InlineString>> OthersParser { get; private set; }

        public string StartsWithChars
        {
            get { return null; }
        }

        public bool CanParse(Subject subject)
        {
            return true;
        }

        public InlineString Parse(ParserContext context, Subject subject)
        {
            var inline = Parsers.EscapedCharParser.Parse(context, subject);
            if (inline == null)
            {
                inline = OthersParser.Value.Parse(context, subject);
            }
            return inline;
        }
    }
}
