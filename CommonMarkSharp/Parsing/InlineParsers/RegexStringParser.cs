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
    public class RegexStringParser : RegexParser<InlineString>
    {
        public RegexStringParser(string pattern, params string[] args)
            : base(pattern, args)
        {
        }

        protected RegexStringParser(Regex re)
            : base(re)
        {
        }

        protected override InlineString HandleMatch(ParserContext context, string[] groups)
        {
            return new InlineString(groups[0]);
        }
    }
}
