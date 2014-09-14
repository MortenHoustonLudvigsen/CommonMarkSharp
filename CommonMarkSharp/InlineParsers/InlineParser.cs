using CommonMarkSharp.Inlines;
using System;
using System.Linq;

namespace CommonMarkSharp.InlineParsers
{
    public class InlineParser : IInlineParser<Inline>
    {
        public InlineParser(Parsers parsers)
        {
            Parsers = parsers;
            _startsWithChars = new Lazy<string>(() => new string(GetStartsWithChars().Distinct().ToArray()));
        }

        protected virtual string GetStartsWithChars()
        {
            return  Parsers.LineBreakParser.StartsWithChars +
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
            return  (Inline)Parsers.LineBreakParser.Parse(context, subject) ??
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
