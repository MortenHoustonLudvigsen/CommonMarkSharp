using CommonMarkSharp.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.InlineParsers
{
    public class AllExceptParser : IParser<InlineString>
    {
        public AllExceptParser(string significantChars)
        {
            SignificantChars = significantChars;
        }

        public string SignificantChars { get; private set; }
        public string StartsWithChars { get { return null; } }

        public InlineString Parse(ParserContext context, Subject subject)
        {
            if (SignificantChars != null)
            {
                var chars = subject.TakeWhile(c => !SignificantChars.Contains(c));
                if (chars.Any())
                {
                    return new InlineString(chars);
                }
            }
            return null;
        }
    }
}
