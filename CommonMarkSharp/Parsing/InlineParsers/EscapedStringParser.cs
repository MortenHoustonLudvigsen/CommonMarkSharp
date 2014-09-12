using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Parsing.InlineParsers
{
    public class EscapedStringParser : CompositeInlineParser
    {
        public EscapedStringParser(Parsers parsers)
            : base(true)
        {
            Parsers = parsers;
        }
    
        public Parsers Parsers { get; private set; }

        protected override void Initialize()
        {
            Register(Parsers.EscapedCharParser);
        }
    }
}
