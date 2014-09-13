using CommonMarkSharp.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.InlineParsers
{
    public class StrWithEntitiesParser : CompositeInlineParser
    {
        public StrWithEntitiesParser(Parsers parsers)
            : base(true)
        {
            Parsers = parsers;
        }

        public Parsers Parsers { get; private set; }

        protected override void Initialize()
        {
            Register(Parsers.EntityParser);
        }
    }
}
