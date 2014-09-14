using CommonMarkSharp.Blocks;

namespace CommonMarkSharp.BlockParsers
{
    public class IndentedCodeParser : IBlockParser<IndentedCode>
    {
        public bool Parse(ParserContext context, Subject subject)
        {
            // Do not match if in a paragraph
            if (context.Tip is Paragraph) return false;

            // Do not match blank line
            if (subject.IsBlank) return false;

            if (subject.Indent >= CommonMarkParser.CODE_INDENT)
            {
                // indented code
                subject.Advance(CommonMarkParser.CODE_INDENT);
                context.CloseUnmatchedBlocks();
                context.Container = context.AddBlock(new IndentedCode());
                return true;
            }

            return false;
        }
    }
}
