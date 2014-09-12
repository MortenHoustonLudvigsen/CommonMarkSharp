using CommonMarkSharp.Parsing.Blocks;
using CommonMarkSharp.Parsing.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonMarkSharp.Parsing.InlineParsers
{
    public class LinkParser : IParser<Link>
    {
        public LinkParser(Parsers parsers)
        {
            Parsers = parsers;
        }

        public Parsers Parsers { get; private set; }

        public string StartsWithChars { get { return "["; } }

        public Link Parse(ParserContext context, Subject subject)
        {
            if (!this.CanParse(subject)) return null;

            var savedSubject = subject.Save();

            var label = Parsers.LinkLabelParser.Parse(context, subject);
            if (label != null && subject.Char == '(')
            {
                subject.Advance();
                subject.SkipWhiteSpace();

                var destination = Parsers.LinkDestinationParser.Parse(context, subject);
                var advanced = subject.SkipWhiteSpace();

                var title = new LinkTitle();
                if (destination == null || advanced > 0)
                {
                    title = Parsers.LinkTitleParser.Parse(context, subject);
                    subject.SkipWhiteSpace();
                }

                if (subject.Char == ')')
                {
                    subject.Advance();
                    return new Link(label, destination, title);
                }
            }

            savedSubject.Restore();
            return null;
        }
    }
}
