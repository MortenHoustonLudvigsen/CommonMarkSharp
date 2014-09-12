using CommonMarkSharp.Parsing.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Parsing.InlineParsers
{
    public class InlineParser : CompositeInlineParser
    {
        public InlineParser(Parsers parsers, bool parseOthersAsStrings = false)
            : base(parseOthersAsStrings)
        {
            Parsers = parsers;
        }

        public Parsers Parsers { get; private set; }

        protected override void Initialize()
        {
            Register(
                Parsers.LineBreakParser,
                Parsers.InlineCodeParser,
                Parsers.BacktickParser,
                Parsers.EscapedCharParser,
                Parsers.EntityParser,
                Parsers.EmphasisParser,
                Parsers.AutolinkParser,
                Parsers.AutolinkEmailParser,
                Parsers.RawHtmlParser,
                Parsers.LinkParser,
                Parsers.LinkReferenceParser,
                Parsers.ImageParser,
                Parsers.ImageReferenceParser
            );
        }
    }
}
