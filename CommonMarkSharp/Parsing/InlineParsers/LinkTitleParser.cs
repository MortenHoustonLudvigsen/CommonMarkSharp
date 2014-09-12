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
    public class LinkTitleParser : IParser<LinkTitle>
    {
        private Lazy<IParser<Inline>> _doubleQuoteContentParser;
        private Lazy<IParser<Inline>> _singleQuoteContentParser;
        private Lazy<IParser<Inline>> _paranthesesContentParser;

        public LinkTitleParser(Parsers parsers)
        {
            _doubleQuoteContentParser = new Lazy<IParser<Inline>>(() => new CompositeInlineParser(
                parsers.EntityParser,
                parsers.EscapedCharParser,
                new RegexStringParser(@"\G[^""\\]")
            ));

            _singleQuoteContentParser = new Lazy<IParser<Inline>>(() => new CompositeInlineParser(
                parsers.EntityParser,
                parsers.EscapedCharParser,
                new RegexStringParser(@"\G[^'\\]")
            ));

            _paranthesesContentParser = new Lazy<IParser<Inline>>(() => new CompositeInlineParser(
                parsers.EntityParser,
                parsers.EscapedCharParser,
                new RegexStringParser(@"\G[^)\\]")
            ));
        }

        public string StartsWithChars { get { return "\"'("; } }

        public LinkTitle Parse(ParserContext context, Subject subject)
        {
            if (!this.CanParse(subject)) return null;

            var endChar = char.MinValue;
            var contentParser = (IParser<Inline>)null;

            if (subject.Char == '"')
            {
                endChar = '"';
                contentParser = _doubleQuoteContentParser.Value;
            }
            else if (subject.Char == '\'')
            {
                endChar = '\'';
                contentParser = _singleQuoteContentParser.Value;
            }
            else if (subject.Char == '(')
            {
                endChar = ')';
                contentParser = _paranthesesContentParser.Value;
            }
            else
            {
                return null;
            }

            var savedSubject = subject.Save();
            subject.Advance();
            var inlines = contentParser.ParseMany(context, subject);

            if (subject.Char == endChar)
            {
                subject.Advance();
                return new LinkTitle(inlines);
            }

            savedSubject.Restore();
            return null;
        }
    }
}
