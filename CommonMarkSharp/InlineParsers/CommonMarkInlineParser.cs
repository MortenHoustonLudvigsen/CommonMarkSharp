using CommonMarkSharp.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.InlineParsers
{
    public class CommonMarkInlineParser : IParser<Inline>
    {
        public CommonMarkInlineParser(Parsers parsers)
        {
            Parsers = parsers;
            OthersParser = new Lazy<IParser<InlineString>>(() => new AllExceptParser(parsers.InlineParser.StartsWithChars));
        }

        public Parsers Parsers { get; private set; }
        public Lazy<IParser<InlineString>> OthersParser { get; private set; }

        public string StartsWithChars
        {
            get { return null; }
        }

        public bool CanParse(Subject subject)
        {
            return true;
        }

        public Inline Parse(ParserContext context, Subject subject)
        {
            var inline = Parsers.InlineParser.Parse(context, subject);
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
