using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Parsing.Inlines
{
    public class InlineRawHtml : Inline
    {
        public InlineRawHtml(string html)
        {
            Html = html;
        }

        public string Html { get; private set; }

        public override string ToString()
        {
            return Html;
        }
    }
}
