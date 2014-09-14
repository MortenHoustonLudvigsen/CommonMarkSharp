using CommonMarkSharp.Blocks;
using CommonMarkSharp.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonMarkSharp.InlineParsers
{
    public class LinkParser : IInlineParser<Link>
    {
        public LinkParser(Parsers parsers)
        {
            Parsers = parsers;
        }

        public Parsers Parsers { get; private set; }

        public string StartsWithChars { get { return "["; } }

        public bool CanParse(Subject subject)
        {
            return subject.Char == '[';
        }

        public Link Parse(ParserContext context, Subject subject)
        {
            if (!CanParse(subject)) return null;

            var saved = subject.Save();

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

            saved.Restore();
            return null;
        }
    }
}
