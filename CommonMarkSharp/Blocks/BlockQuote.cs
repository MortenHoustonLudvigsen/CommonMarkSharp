using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Blocks
{
    public class BlockQuote : ContainerBlock
    {
        public override bool CanContain(Block block)
        {
            return true;
        }

        public override bool MatchNextLine(LineInfo lineInfo)
        {
            var matched = lineInfo.Indent <= 3 && lineInfo[lineInfo.FirstNonSpace] == '>';
            if (matched)
            {
                lineInfo.Offset = lineInfo.FirstNonSpace + 1;
                if (lineInfo[lineInfo.Offset] == ' ')
                {
                    lineInfo.Offset++;
                }
            }
            return matched;
        }
    }
}
