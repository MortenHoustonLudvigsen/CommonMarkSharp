using CommonMarkSharp.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.BlockParsers
{
    public class ATXHeaderParser : IBlockParser<ATXHeader>
    {
        public string StartsWithChars
        {
            get { return " #"; }
        }

        public bool CanParse(Subject subject)
        {
            return subject.FirstNonSpaceChar == '#';
        }

        public bool Parse(ParserContext context, Subject subject)
        {
            if (!CanParse(subject)) return false;

            var saved = subject.Save();

            subject.AdvanceWhile(c => c == ' ', 3);

            var level = subject.AdvanceWhile(c => c == '#');
            var contents = new StringBuilder();

            if (level >= 1 && level <= 6 && (subject.Char == ' ' || subject.EndOfString))
            {
                subject.AdvanceToFirstNonSpace();
                while (!subject.EndOfString)
                {
                    if (subject.Char == '\\')
                    {
                        contents.Append(subject.Take());
                        if (!subject.EndOfString)
                        {
                            contents.Append(subject.Take());
                        }
                    }
                    else if (subject.Char == '#')
                    {
                        var closingSequence = subject.TakeWhile(c => c == '#');
                        var closingSpace = subject.TakeWhile(c => c == ' ');
                        if (!subject.EndOfString)
                        {
                            contents.Append(closingSequence);
                            contents.Append(closingSpace);
                        }
                    }
                    else
                    {
                        contents.Append(subject.TakeWhile(c => c != '#' && c != '\\' && c != ' '));
                        var closingSpace = subject.TakeWhile(c => c == ' ');
                        if (!subject.EndOfString)
                        {
                            contents.Append(closingSpace);
                        }
                    }
                }
                context.AddBlock(new ATXHeader(level, contents.ToString()));
                context.BlocksParsed = true;
                return true;
            }

            saved.Restore();
            return false;
        }
    }
}
