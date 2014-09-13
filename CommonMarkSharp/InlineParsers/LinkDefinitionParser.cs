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
    public class LinkDefinitionParser : IParser<Link>
    {
        public LinkDefinitionParser(Parsers parsers)
        {
            Parsers = parsers;
        }

        public Parsers Parsers { get; private set; }

        public string StartsWithChars { get { return " ["; } }

        public Link Parse(ParserContext context, Subject subject)
        {
            if (!this.CanParse(subject)) return null;

            var savedSubject = subject.Save();

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
                    savedSubject.Restore();
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

            savedSubject.Restore();
            return null;
        }
    }
}
