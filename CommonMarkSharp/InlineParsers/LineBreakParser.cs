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
    public class LineBreakParser : IParser<LineBreak>
    {
        private static readonly Regex _startsWithDoubleSpaceNewLine = RegexUtils.Create(@"\G  +\n");
        public string StartsWithChars { get { return "\\ \n"; } }

        public LineBreak Parse(ParserContext context, Subject subject)
        {
            if (!this.CanParse(subject)) return null;

            string[] groups;
            if (subject.IsMatch(_startsWithDoubleSpaceNewLine, 0, out groups))
            {
                subject.Advance(groups[0].Length);
                subject.AdvanceWhile(c => c == ' ');
                return new HardBreak();
            }
            else if (subject.StartsWith("\\\n"))
            {
                subject.Advance(2);
                subject.AdvanceWhile(c => c == ' ');
                return new HardBreak();
            }
            else if (subject.StartsWith(" \n"))
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
