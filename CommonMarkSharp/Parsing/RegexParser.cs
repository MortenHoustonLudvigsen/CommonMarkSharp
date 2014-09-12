using CommonMarkSharp.Parsing.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonMarkSharp.Parsing
{
    public abstract class RegexParser<TPart> : IParser<TPart>
        where TPart : Part
    {
        public RegexParser(string pattern, params string[] args)
            : this(RegexUtils.Create(pattern, args))
        {
        }

        protected RegexParser(Regex re)
        {
            Re = re;
        }

        public Regex Re { get; private set; }

        protected abstract TPart HandleMatch(ParserContext context, string[] groups);

        public string StartsWithChars { get; set; }

        public TPart Parse(ParserContext context, Subject subject)
        {
            if (!this.CanParse(subject)) return null;

            string[] groups;
            if (subject.IsMatch(Re, 0, out groups))
            {
                subject.Advance(groups[0].Length);
                return HandleMatch(context, groups);
            }
            return null;
        }
    }
}
