using CommonMarkSharp.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.InlineParsers
{
    public class AllParser : IParser<InlineString>
    {
        public AllParser(string chars)
        {
            StartsWithChars = chars;
        }

        public string StartsWithChars { get; private set; }

        public InlineString Parse(ParserContext context, Subject subject)
        {
            if (StartsWithChars != null)
            {
                var chars = subject.TakeWhile(c => StartsWithChars.Contains(c));
                if (chars.Any())
                {
                    return new InlineString(chars);
                }
            }
            return null;
        }
    }
}
