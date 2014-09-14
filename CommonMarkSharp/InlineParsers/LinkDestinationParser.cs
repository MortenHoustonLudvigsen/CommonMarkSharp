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
    // TODO: Clean up
    public class LinkDestinationParser : IInlineParser<LinkDestination>
    {
        private Lazy<IInlineParser<Inline>> _braceContentParser;
        private Lazy<IInlineParser<Inline>> _destContentParser;
        private Lazy<IInlineParser<Inline>> _destContentParserWithParantheses;

        public LinkDestinationParser(Parsers parsers)
        {
            _braceContentParser = new Lazy<IInlineParser<Inline>>(() => new CompositeParser<Inline>(
                parsers.EntityParser,
                parsers.EscapedCharParser,
                new AllExceptParser(@"<>\")
            ));

            _destContentParser = new Lazy<IInlineParser<Inline>>(() => new CompositeParser<Inline>(
                parsers.EntityParser,
                parsers.EscapedCharParser,
                new AllExceptParser(Patterns.ControlChars + @"()\&")
            ));

            _destContentParserWithParantheses = new Lazy<IInlineParser<Inline>>(() => new CompositeParser<Inline>(
                parsers.EntityParser,
                parsers.EscapedCharParser,
                new DestParanthesesParser(_destContentParser.Value),
                new AllExceptParser(Patterns.ControlChars + @"()\&")
            ));
        }

        public string StartsWithChars { get { return null; } } // Any character

        public bool CanParse(Subject subject)
        {
            return true;
        }

        public LinkDestination Parse(ParserContext context, Subject subject)
        {
            if (!CanParse(subject)) return null;

            var savedSubject = subject.Save();
            var inlines = new List<Inline>();

            if (subject.Char == '<')
            {
                subject.Advance();
                inlines.AddRange(_braceContentParser.Value.ParseMany(context, subject));
                if (subject.Char == '>')
                {
                    subject.Advance();
                    return new LinkDestination(inlines);
                }
            }
            else
            {
                var content = _destContentParserWithParantheses.Value.Parse(context, subject);
                if (content != null)
                {
                    inlines.Add(content);
                    inlines.AddRange(_destContentParserWithParantheses.Value.ParseMany(context, subject));
                    return new LinkDestination(inlines);
                }
            }

            savedSubject.Restore();
            return null;
        }

        public class DestParanthesesParser : IInlineParser<InlineList>
        {
            public DestParanthesesParser(IInlineParser<Inline> contentParser)
            {
                ContentParser = contentParser;
            }

            public IInlineParser<Inline> ContentParser { get; private set; }

            public string StartsWithChars { get { return "("; } }

            public bool CanParse(Subject subject)
            {
                return subject.Char == '(';
            }

            public InlineList Parse(ParserContext context, Subject subject)
            {
                if (subject.Char != '(')
                {
                    return null;
                }
                var savedSubject = subject.Save();

                var inlines = new List<Inline>();
                inlines.Add(new InlineString('('));
                subject.Advance();
                inlines.AddRange(ContentParser.ParseMany(context, subject));

                if (subject.Char == ')')
                {
                    subject.Advance();
                    inlines.Add(new InlineString(')'));
                    return new InlineList(inlines);
                }

                savedSubject.Restore();
                return null;
            }
        }
    }
}
