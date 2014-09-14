using CommonMarkSharp.Blocks;
using CommonMarkSharp.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.InlineParsers
{
    public class EscapedCharParser : IInlineParser<InlineString>
    {
        public static readonly HashSet<char> PunctuationChars = new HashSet<char>(@"!""#$%&'()*+,-./:;<=>?@[\]^_`{|}~");

        public string StartsWithChars { get { return "\\"; } }

        public bool CanParse(Subject subject)
        {
            return subject.Char == '\\';
        }

        public InlineString Parse(ParserContext context, Subject subject)
        {
            if (!CanParse(subject)) return null;

            if (subject.Char == '\\' && PunctuationChars.Contains(subject[1]))
            {
                subject.Advance(2);
                return new InlineString(subject[-1]);
            }
            return null;
        }
    }
}
