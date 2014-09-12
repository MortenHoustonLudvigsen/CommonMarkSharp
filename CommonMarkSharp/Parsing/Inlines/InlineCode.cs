using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Parsing.Inlines
{
    public class InlineCode : Inline
    {
        public InlineCode(string code)
        {
            Code = code.Trim();
        }

        public string Code { get; private set; }

        public override string ToString()
        {
            return string.Format("```{0}```", Code);
        }
    }
}
