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
    public class LinkTitleParser : IInlineParser<LinkTitle>
    {
        private Lazy<IInlineParser<Inline>> _doubleQuoteContentParser;
        private Lazy<IInlineParser<Inline>> _singleQuoteContentParser;
        private Lazy<IInlineParser<Inline>> _paranthesesContentParser;

        public LinkTitleParser(Parsers parsers)
        {
            _doubleQuoteContentParser = new Lazy<IInlineParser<Inline>>(() => new CompositeInlineParser<Inline>(
                parsers.EntityParser,
                parsers.EscapedCharParser,
                new AllExceptParser(@"""\", 1)
                //new RegexStringParser(@"\G[^""\\]")
            ));

            _singleQuoteContentParser = new Lazy<IInlineParser<Inline>>(() => new CompositeInlineParser<Inline>(
                parsers.EntityParser,
                parsers.EscapedCharParser,
                new AllExceptParser(@"'\", 1)
                //new RegexStringParser(@"\G[^'\\]")
            ));

            _paranthesesContentParser = new Lazy<IInlineParser<Inline>>(() => new CompositeInlineParser<Inline>(
                parsers.EntityParser,
                parsers.EscapedCharParser,
                new AllExceptParser(@")\", 1)
                //new RegexStringParser(@"\G[^)\\]")
            ));
        }

        public string StartsWithChars { get { return "\"'("; } }

        public bool CanParse(Subject subject)
        {
            return subject.Char == '"' || subject.Char == '\'' || subject.Char == '(';
        }

        public LinkTitle Parse(ParserContext context, Subject subject)
        {
            if (!CanParse(subject)) return null;

            var endChar = char.MinValue;
            var contentParser = (IInlineParser<Inline>)null;

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

            var saved = subject.Save();
            subject.Advance();
            var inlines = contentParser.ParseMany(context, subject);

            if (subject.Char == endChar)
            {
                subject.Advance();
                return new LinkTitle(inlines);
            }

            saved.Restore();
            return null;
        }
    }
}
