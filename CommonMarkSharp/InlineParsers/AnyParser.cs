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
    public class AnyParser : IParser<InlineString>
    {
        public AnyParser(string significantChars)
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
            return new InlineString(subject.Take());
        }
    }
}
