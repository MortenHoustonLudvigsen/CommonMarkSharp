using CommonMarkSharp.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonMarkSharp.InlineParsers
{
    public class LinkLabelParser : IInlineParser<LinkLabel>
    {
        private Lazy<IInlineParser<Inline>> _contentParser;

        public LinkLabelParser(Parsers parsers)
        {
            _contentParser = new Lazy<IInlineParser<Inline>>(() => new CompositeInlineParser<Inline>(
                parsers.InlineCodeParser,
                parsers.AutolinkParser,
                parsers.AutolinkEmailParser,
                parsers.RawHtmlParser,
                parsers.EmphasisParser,
                this,
                parsers.EscapedCharParser,
                new AllExceptParser(@"]", 1)
            ));
        }

        public string StartsWithChars { get { return "["; } }

        public bool CanParse(Subject subject)
        {
            return subject.Char == '[';
        }

        public LinkLabel Parse(ParserContext context, Subject subject)
        {
            if (!CanParse(subject)) return null;

            var embedded = context.HasParam(typeof(LinkLabel));
            var inlines = new List<Inline>();
            var saved = subject.Save();
            subject.Advance();
            var subjectLiteral = subject.Save();
            if (embedded)
            {
                inlines.Add(new InlineString("["));
            }
            inlines.AddRange(_contentParser.Value.ParseMany(context.Add(typeof(LinkLabel)), subject));

            if (subject.Char == ']')
            {
                var literal = subjectLiteral.GetLiteral();
                subject.Advance();
                if (embedded)
                {
                    inlines.Add(new InlineString("]"));
                }
                return new LinkLabel(literal, inlines);
            }

            saved.Restore();
            return null;
        }

        public class ContentParser : IInlineParser<Inline>
        {
            public ContentParser(Parsers parsers, LinkLabelParser linkLabelParser)
            {
                Parsers = parsers;
                _startsWithChars = new Lazy<string>(() => new string(GetStartsWithChars().Distinct().ToArray()));
            }

            protected virtual string GetStartsWithChars()
            {
                return Parsers.LineBreakParser.StartsWithChars +
                       Parsers.InlineCodeParser.StartsWithChars +
                       Parsers.BacktickParser.StartsWithChars +
                       Parsers.EscapedCharParser.StartsWithChars +
                       Parsers.EntityParser.StartsWithChars +
                       Parsers.EmphasisParser.StartsWithChars +
                       Parsers.AutolinkParser.StartsWithChars +
                       Parsers.AutolinkEmailParser.StartsWithChars +
                       Parsers.RawHtmlParser.StartsWithChars +
                       Parsers.LinkParser.StartsWithChars +
                       Parsers.LinkReferenceParser.StartsWithChars +
                       Parsers.ImageParser.StartsWithChars +
                       Parsers.ImageReferenceParser.StartsWithChars;
            }

            public Parsers Parsers { get; private set; }

            private Lazy<string> _startsWithChars;
            public string StartsWithChars
            {
                get { return _startsWithChars.Value; }
            }

            public bool CanParse(Subject subject)
            {
                return StartsWithChars.Contains(subject.Char);
            }

            public Inline Parse(ParserContext context, Subject subject)
            {
                return (Inline)Parsers.LineBreakParser.Parse(context, subject) ??
                        (Inline)Parsers.InlineCodeParser.Parse(context, subject) ??
                        (Inline)Parsers.BacktickParser.Parse(context, subject) ??
                        (Inline)Parsers.EscapedCharParser.Parse(context, subject) ??
                        (Inline)Parsers.EntityParser.Parse(context, subject) ??
                        (Inline)Parsers.EmphasisParser.Parse(context, subject) ??
                        (Inline)Parsers.AutolinkParser.Parse(context, subject) ??
                        (Inline)Parsers.AutolinkEmailParser.Parse(context, subject) ??
                        (Inline)Parsers.RawHtmlParser.Parse(context, subject) ??
                        (Inline)Parsers.LinkParser.Parse(context, subject) ??
                        (Inline)Parsers.LinkReferenceParser.Parse(context, subject) ??
                        (Inline)Parsers.ImageParser.Parse(context, subject) ??
                        (Inline)Parsers.ImageReferenceParser.Parse(context, subject);
            }
        }
    }
}
