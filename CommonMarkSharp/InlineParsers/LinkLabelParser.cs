using CommonMarkSharp.Blocks;
using CommonMarkSharp.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.InlineParsers
{
    public class LinkLabelParser : IParser<LinkLabel>
    {
        private Lazy<IParser<Inline>> _contentParser;

        public LinkLabelParser(Parsers parsers)
        {
            _contentParser = new Lazy<IParser<Inline>>(() => new CompositeInlineParser(
                parsers.InlineCodeParser,
                parsers.AutolinkParser,
                parsers.AutolinkEmailParser,
                parsers.RawHtmlParser,
                parsers.EmphasisParser,
                this,
                parsers.EscapedCharParser,
                new RegexStringParser(@"\G[^\]]")
            ));
        }

        public string StartsWithChars { get { return "["; } }

        public LinkLabel Parse(ParserContext context, Subject subject)
        {
            if (!this.CanParse(subject)) return null;

            var embedded = context.HasParam(typeof(LinkLabel));
            var inlines = new List<Inline>();
            var savedSubject = subject.Save();
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

            savedSubject.Restore();
            return null;
        }
    }
}
