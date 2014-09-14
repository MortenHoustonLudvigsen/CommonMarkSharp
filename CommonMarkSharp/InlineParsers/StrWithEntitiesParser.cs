using CommonMarkSharp.Inlines;
using System;

namespace CommonMarkSharp.InlineParsers
{
    public class StrWithEntitiesParser : IInlineParser<InlineString>
    {
        public StrWithEntitiesParser(Parsers parsers)
        {
            Parsers = parsers;
            OthersParser = new Lazy<IInlineParser<InlineString>>(() => new AllExceptParser(parsers.EntityParser.StartsWithChars));
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
            var inline = (InlineString)Parsers.EntityParser.Parse(context, subject);
            if (inline == null)
            {
                inline = OthersParser.Value.Parse(context, subject);
            }
            if (inline == null)
            {
                inline = new InlineString(subject.Take());
            }
            return inline;
        }
    }
}
