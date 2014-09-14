using CommonMarkSharp.Blocks;

namespace CommonMarkSharp.BlockParsers
{
    public class HorizontalRuleParser : IBlockParser<HorizontalRule>
    {
        public bool Parse(ParserContext context, Subject subject)
        {
            var saved = subject.Save();

            subject.AdvanceWhile(c => c == ' ', 3);

            if (subject.Char == '-' || subject.Char == '_' || subject.Char == '*')
            {
                var hRuleChar = subject.Char;
                var length = subject.AdvanceWhile(c => c == hRuleChar);
                subject.SkipWhiteSpace();
                while (subject.Char == hRuleChar)
                {
                    length += subject.AdvanceWhile(c => c == hRuleChar);
                    subject.SkipWhiteSpace();
                }
                if (length >= 3)
                {
                    subject.SkipWhiteSpace();
                    if (subject.EndOfString)
                    {
                        context.AddBlock(new HorizontalRule());
                        context.BlocksParsed = true;
                        return true;
                    }
                }
            }

            saved.Restore();
            return false;
        }
    }
}
