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
    public class EntityParser : IInlineParser<Entity>
    {
        public string StartsWithChars
        {
            get { return "&"; }
        }

        public bool CanParse(Subject subject)
        {
            return subject.Char == '&';
        }

        public Entity Parse(ParserContext context, Subject subject)
        {
            if (!CanParse(subject)) return null;

            var saved = subject.Save();
            subject.Advance();

            var found = false;

            if (subject.Char == '#')
            {
                subject.Advance();
                if (subject.Char == 'x' || subject.Char == 'X')
                {
                    subject.Advance();
                    found = subject.AdvanceWhile(c => Patterns.HexDigits.Contains(c), 8) >= 1;
                }
                else if (Patterns.Digits.Contains(subject.Char))
                {
                    found = subject.AdvanceWhile(c => Patterns.Digits.Contains(c), 8) >= 1;
                }
            }
            else if (Patterns.Alphas.Contains(subject.Char))
            {
                found = subject.AdvanceWhile(c => Patterns.Alphanums.Contains(c), 32) >= 2;
            }

            if (found && subject.Char == ';')
            {
                subject.Advance();
                return new Entity(saved.GetLiteral());
            }

            saved.Restore();
            return null;
        }
    }
}
