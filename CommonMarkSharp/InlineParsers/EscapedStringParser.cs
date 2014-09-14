using CommonMarkSharp.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.InlineParsers
{
    public class EscapedStringParser : IParser<InlineString>
    {
        public EscapedStringParser(Parsers parsers)
        {
            Parsers = parsers;
            OthersParser = new Lazy<IParser<InlineString>>(() => new AllExceptParser(parsers.EscapedCharParser.StartsWithChars));
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
