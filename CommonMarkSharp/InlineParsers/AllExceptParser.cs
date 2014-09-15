using CommonMarkSharp.Inlines;
using System.Collections.Generic;
using System.Linq;

namespace CommonMarkSharp.InlineParsers
{
    public class AllExceptParser : IInlineParser<InlineString>
    {
        public AllExceptParser(CharSet significantChars, int max = int.MaxValue)
        {
            SignificantChars = significantChars;
            Max = max;
        }

        public CharSet SignificantChars { get; private set; }
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
                var chars = subject.TakeWhile(c => !SignificantChars.Contains(c), Max);
                if (chars.Any())
                {
                    return new InlineString(chars);
                }
            }
            return null;
        }
    }
}
