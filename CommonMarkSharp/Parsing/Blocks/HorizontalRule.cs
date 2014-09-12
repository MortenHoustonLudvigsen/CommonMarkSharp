using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Parsing.Blocks
{
    public class HorizontalRule : LeafBlock
    {
        public override bool MatchNextLine(LineInfo lineInfo)
        {
            return false;
        }
    }
}
