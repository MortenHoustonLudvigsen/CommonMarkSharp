using CommonMarkSharp.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonMarkSharp.InlineParsers
{
    public class AllExceptParser : IInlineParser<InlineString>
    {
        public AllExceptParser(CharSet significantChars, int max = int.MaxValue)
        {
            SignificantChars = significantChars;
            SignificantCharsX = significantChars.Chars.ToArray();
            Max = max;
        }

        public CharSet SignificantChars { get; private set; }
        public char[] SignificantCharsX { get; private set; }
        public string StartsWithChars { get { return null; } }
        public int Max { get; private set; }

        public bool CanParse(Subject subject)
        {
            return SignificantChars.Any && !SignificantChars.Contains(subject.Char);
        }

        public InlineString Parse(ParserContext context, Subject subject)
        {
            if (SignificantChars.Any)
            {
                var count = subject.CountWhileNot(SignificantChars, Max);
                if (count > 0)
                {
                    return new InlineString(subject.Take(count));
                }
            }
            return null;
        }
    }
}
