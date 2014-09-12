using CommonMarkSharp.Parsing.Blocks;
using CommonMarkSharp.Parsing.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonMarkSharp.Parsing.InlineParsers
{
    public class InlineCodeParser : IParser<InlineCode>
    {
        public string StartsWithChars { get { return "`"; } }

        public InlineCode Parse(ParserContext context, Subject subject)
        {
            if (!this.CanParse(subject)) return null;

            var savedSubject = subject.Save();
            var openticks = subject.TakeWhile(() => subject.Char == '`').ToArray();
            var code = "";
            var codeEnded = false;
            while (!subject.EndOfString && !codeEnded)
            {
                if (subject.Char == '`')
                {
                    var closeticks = subject.TakeWhile(() => subject.Char == '`').ToArray();
                    if (closeticks.Length == openticks.Length)
                    {
                        codeEnded = true;
                    }
                    else
                    {
                        code += new string(closeticks);
                    }
                }
                else
                {
                    code += new string(subject.TakeWhile(() => subject.Char != '`').ToArray());
                }
            }
            if (codeEnded)
            {
                return new InlineCode(NormalizeWhitespace(code));
            }

            savedSubject.Restore();
            return null;
        }

        protected static string NormalizeWhitespace(string text)
        {
            return Regex.Replace(text, @"\s+", " ");
        }
    }
}
