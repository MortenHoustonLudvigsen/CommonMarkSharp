using CommonMarkSharp.Blocks;
using CommonMarkSharp.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.InlineParsers
{
    public class BacktickParser : IInlineParser<InlineString>
    {
        public string StartsWithChars { get { return "`"; } }

        public bool CanParse(Subject subject)
        {
            return subject.Char == '`';
        }

        public InlineString Parse(ParserContext context, Subject subject)
        {
            if (!CanParse(subject)) return null;

            return new InlineString(subject.TakeWhile(c => c == '`'));
        }
    }
}
