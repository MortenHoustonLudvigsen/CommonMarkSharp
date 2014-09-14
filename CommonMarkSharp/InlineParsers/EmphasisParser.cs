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
    public class EmphasisParser : IInlineParser<Inline>
    {
        public EmphasisParser(Parsers parsers)
        {
            Parsers = parsers;
        }

        public Parsers Parsers { get; private set; }

        public string StartsWithChars { get { return "*_"; } }

        public bool CanParse(Subject subject)
        {
            return subject.Char == '*' || subject.Char == '_';
        }

        public Inline Parse(ParserContext context, Subject subject)
        {
            if (!CanParse(subject)) return null;

            var saved = subject.Save();
            var emphChar = subject.Char;
            int startCount;
            int endCount;

            if (CanOpen(subject, emphChar, out startCount))
            {
                subject.Advance(startCount);
                var inlines = new List<Inline>();
                while (!subject.EndOfString)
                {
                    if (CanClose(subject, emphChar, startCount, out endCount))
                    {
                        if (startCount == 1)
                        {
                            subject.Advance(1);
                            return new Emphasis(inlines);
                        }
                        else if (startCount == 2)
                        {
                            subject.Advance(2);
                            return new StrongEmphasis(inlines);
                        }
                        else
                        {
                            subject.Advance(endCount);
                            if (endCount == 1)
                            {
                                var emphasis = new Emphasis(inlines);
                                inlines.Clear();
                                inlines.Add(emphasis);
                                startCount = 2;
                            }
                            else if (endCount == 2)
                            {
                                var strongEmphasis = new StrongEmphasis(inlines);
                                inlines.Clear();
                                inlines.Add(strongEmphasis);
                                startCount = 1;
                            }
                            else
                            {
                                return new StrongEmphasis(new Emphasis(inlines));
                            }
                        }
                    }
                    if (subject.EndOfString)
                    {
                        break;
                    }
                    else
                    {
                        var inline = Parsers.CommonMarkInlineParser.Parse(context, subject);
                        if (inline == null)
                        {
                            saved.Restore();
                            return null;
                        }
                        inlines.Add(inline);
                    }
                }
            }

            saved.Restore();
            return new InlineString(subject.TakeWhile(c => c == emphChar));
        }

        private bool CanOpen(Subject subject, char emphChar, out int count)
        {
            count = subject.CountWhile(c => c == emphChar);

            if (subject.Char != emphChar)
            {
                return false;
            }
            if (count > 3)
            {
                return false;
            }
            if (subject.PartOfSequence(emphChar, 4))
            {
                return false;
            }
            if (subject.IsWhiteSpace(count))
            {
                return false;
            }
            if (emphChar == '_' && Patterns.Alphanums.Contains(subject[-1]))
            {
                return false;
            }
            return true;
        }

        private bool CanClose(Subject subject, char emphChar, int startCount, out int count)
        {
            count = subject.CountWhile(c => { return c == emphChar; });

            if (subject.Char != emphChar)
            {
                return false;
            }
            if (count > 3)
            {
                return false;
            }
            if (startCount == 2 && count < 2)
            {
                return false;
            }
            if (subject.PartOfSequence(emphChar, 4))
            {
                return false;
            }
            if (subject.IsWhiteSpace(-1))
            {
                return false;
            }

            if (emphChar == '_' && Patterns.Alphanums.Contains(subject[count]))
            {
                return false;
            }
            return true;
        }
    }
}
