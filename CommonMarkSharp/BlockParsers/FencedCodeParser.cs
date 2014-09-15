using CommonMarkSharp.Blocks;

namespace CommonMarkSharp.BlockParsers
{
    public class FencedCodeParser : IBlockParser<FencedCode>
    {
        public bool Parse(ParserContext context, Subject subject)
        {
            var saved = subject.Save();

            var fenceOffset = subject.AdvanceWhile(' ', 3);

            if (subject.Char == '`' || subject.Char == '~')
            {
                var fenceChar = subject.Char;
                var fenceLength = subject.AdvanceWhile(fenceChar);
                if (fenceLength >= 3 && !subject.Contains(fenceChar))
                {
                    context.AddBlock(new FencedCode
                    {
                        Length = fenceLength,
                        Char = fenceChar,
                        Offset = fenceOffset
                    });
                    context.BlocksParsed = true;
                    return true;
                }
            }

            saved.Restore();
            return false;
        }
    }
}
