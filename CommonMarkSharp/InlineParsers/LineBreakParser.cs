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
        public string StartsWithChars { get { return "\\ \n"; } }

        public bool CanParse(Subject subject)
        {
            return subject.Char == '\\' || subject.Char == ' ' || subject.Char == '\n';
        }

        public LineBreak Parse(ParserContext context, Subject subject)
        {
            if (!CanParse(subject)) return null;

            var savedSubject = subject.Save();

            LineBreak result = null;

            if (subject.Char == ' ')
            {
                var count = subject.AdvanceWhile(c => c == ' ');
                if (subject.Char == '\n')
                {
                    result = count >= 2 ? (LineBreak)new HardBreak() : new SoftBreak();
                }
            }
            else if (subject.StartsWith("\\\n"))
            {
                subject.Advance(2);
                result = new HardBreak();
            }
            else if (subject.Char == '\n')
            {
                subject.Advance();
                result = new SoftBreak();
            }

            if (result != null)
            {
                subject.SkipWhiteSpace();
                return result;
            }

            savedSubject.Restore();
            return null;
        }
    }
}
