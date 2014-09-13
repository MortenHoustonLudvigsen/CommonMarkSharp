using CommonMarkSharp.Blocks;
using CommonMarkSharp.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.InlineParsers
{
    public class BacktickParser : IParser<InlineString>
    {
        public string StartsWithChars { get { return "`"; } }

        public InlineString Parse(ParserContext context, Subject subject)
        {
            if (!this.CanParse(subject)) return null;

            return new InlineString(subject.TakeWhile(() => subject.Char == '`'));
        }
    }
}
