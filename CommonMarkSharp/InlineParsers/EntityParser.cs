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
    public class EntityParser : IParser<InlineEntity>
    {
        private const string _alphas = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private const string _digits = "0123456789";
        private const string _alphanums = _alphas + _digits;
        private const string _hexDigits = _digits + "ABCDEFabcdef";

        public string StartsWithChars
        {
            get { return "&"; }
        }

        public InlineEntity Parse(ParserContext context, Subject subject)
        {
            if (!this.CanParse(subject)) return null;

            var savedSubject = subject.Save();
            subject.Advance();

            var found = false;

            if (subject.Char == '#')
            {
                subject.Advance();
                if (subject.Char == 'x' || subject.Char == 'X')
                {
                    subject.Advance();
                    found = subject.AdvanceWhile(c => _hexDigits.Contains(c), 8) >= 1;
                }
                else if (_digits.Contains(subject.Char))
                {
                    found = subject.AdvanceWhile(c => _digits.Contains(c), 8) >= 1;
                }
            }
            else if (_alphas.Contains(subject.Char))
            {
                found = subject.AdvanceWhile(c => _alphanums.Contains(c), 32) >= 2;
            }

            if (found && subject.Char == ';')
            {
                subject.Advance();
                return new InlineEntity(savedSubject.GetLiteral());
            }

            savedSubject.Restore();
            return null;
        }
    }
}
