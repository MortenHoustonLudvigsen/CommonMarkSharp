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
        public AnyParser(string significantChars, int max = int.MaxValue)
            : this(new HashSet<char>(significantChars), max)
        {
        }

        public AnyParser(HashSet<char> significantChars, int max = int.MaxValue)
        {
            SignificantChars = significantChars;
            Max = max;
        }

        public HashSet<char> SignificantChars { get; private set; }
        public string StartsWithChars { get { return null; } }
        public int Max { get; private set; }

        public bool CanParse(Subject subject)
        {
            return SignificantChars.Any() && !SignificantChars.Contains(subject.Char);
        }
        
        public InlineString Parse(ParserContext context, Subject subject)
        {
            if (SignificantChars.Any())
            {
                var chars = subject.TakeWhile(c => !SignificantChars.Contains(c), Max);
                if (chars.Any())
                {
                    return new InlineString(chars);
                }
            }
            return new InlineString(subject.Take());
        }
    }
}
