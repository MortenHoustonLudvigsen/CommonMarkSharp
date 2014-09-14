using System.Collections.Generic;
using System.Text;

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
    }
}
