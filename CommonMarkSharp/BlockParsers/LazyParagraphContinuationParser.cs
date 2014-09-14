using CommonMarkSharp.Blocks;

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
