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

        public override bool MatchNextLine(Subject subject)
        {
            var matched = subject.Indent <= 3 && subject.FirstNonSpaceChar == '>';
            if (matched)
            {
                subject.AdvanceToFirstNonSpace();
                subject.Advance();
                if (subject.Char == ' ')
                {
                    subject.Advance();
                }
            }
            return matched;
        }
    }
}
