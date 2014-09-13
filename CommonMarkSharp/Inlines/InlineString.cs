using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Inlines
{
    public class InlineString : Inline
    {
        public InlineString(IEnumerable<char> chars)
            : this(chars.ToArray())
        {
        }

        public InlineString(params char[] chars)
            : this(new string(chars))
        {
        }

        public InlineString(string str)
        {
            Value = str;
        }

        public string Value { get; private set; }

        public override string ToString()
        {
            return Value;
        }
    }
}
