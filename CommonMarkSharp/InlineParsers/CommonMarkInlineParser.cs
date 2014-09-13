using CommonMarkSharp.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.InlineParsers
{
    public class CommonMarkInlineParser : InlineParser
    {
        public CommonMarkInlineParser(Parsers parsers)
            : base(parsers, true)
        {
        }
    }
}
