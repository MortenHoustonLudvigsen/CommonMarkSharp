using CommonMarkSharp.Blocks;
using CommonMarkSharp.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.InlineParsers
{
    public class LineBreakParser : IParser<LineBreak>
    {
        public string StartsWithChars { get { return "\\ \n"; } }

        public LineBreak Parse(ParserContext context, Subject subject)
        {
            if (!this.CanParse(subject)) return null;

            string[] groups;
            if (subject.IsMatch(@"  +\n", 0, out groups))
            {
                subject.Advance(groups[0].Length);
                subject.AdvanceWhile(c => c == ' ');
                return new HardBreak();
            }
            else if (subject.StartsWith("\\\n", 0))
            {
                subject.Advance(2);
                subject.AdvanceWhile(c => c == ' ');
                return new HardBreak();
            }
            else if (subject.StartsWith(" \n", 0))
            {
                subject.Advance(2);
                subject.AdvanceWhile(c => c == ' ');
                return new SoftBreak();
            }
            else if (subject.Char == '\n')
            {
                subject.Advance();
                subject.AdvanceWhile(c => c == ' ');
                return new SoftBreak();
            }

            return null;
        }
    }
}
