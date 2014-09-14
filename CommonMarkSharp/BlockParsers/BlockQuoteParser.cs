using CommonMarkSharp.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.BlockParsers
{
    public class BlockQuoteParser: IBlockParser<BlockQuote>
    {
        public bool Parse(ParserContext context, Subject subject)
        {
            if (subject.FirstNonSpaceChar == '>')
            {
                // blockquote
                subject.AdvanceToFirstNonSpace(1);
                // optional following space
                if (subject.Char == ' ')
                {
                    subject.Advance();
                }
                context.AddBlock(new BlockQuote());
                return true;
            }

            return false;
        }
    }
}
