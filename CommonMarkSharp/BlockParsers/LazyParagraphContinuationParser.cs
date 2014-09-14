using CommonMarkSharp.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.BlockParsers
{
    public class LazyParagraphContinuationParser : IBlockParser<Block>
    {
        public bool Parse(ParserContext context, Subject subject)
        {
            if (subject.Indent >= CommonMarkParser.CODE_INDENT)
            {
                context.BlocksParsed = true;
                return true;
            }
            return false;
        }
    }
}
