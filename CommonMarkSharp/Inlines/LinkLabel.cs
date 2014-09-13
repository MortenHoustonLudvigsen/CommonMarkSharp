using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonMarkSharp.Inlines
{
    public class LinkLabel : InlineList
    {
        public LinkLabel(string literal, IEnumerable<Inline> inlines)
            : base(inlines)
        {
            Literal = Normalize(literal);
        }

        public string Literal { get; private set; }

        public string Normalize(string literal)
        {
            return RegexUtils.NormalizeWhitespace(literal.Normalize(NormalizationForm.FormKD));
        }

        public override string ToString()
        {
            return Literal;
        }
    }
}
