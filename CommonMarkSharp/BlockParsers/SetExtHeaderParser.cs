using CommonMarkSharp.Blocks;
using System.Linq;

namespace CommonMarkSharp.BlockParsers
{
    public class SetExtHeaderParser : IBlockParser<SetExtHeader>
    {
        public bool Parse(ParserContext context, Subject subject)
        {
            if (!(context.Container is Paragraph)) return false;
            if (context.Container.Strings.Count() != 1) return false;

            var saved = subject.Save();

            subject.AdvanceWhile(c => c == ' ', 3);

            if (subject.Char == '-' || subject.Char == '=')
            {
                var headerChar = subject.Char;
                var level = headerChar == '=' ? 1 : 2;

                subject.AdvanceWhile(c => c == headerChar);
                subject.SkipWhiteSpace();
                if (subject.EndOfString)
                {
                    context.CloseUnmatchedBlocks();
                    context.Container = context.Container.Parent.Replace(context, context.Container, new SetExtHeader(level, context.Container.Strings.First()));
                    context.BlocksParsed = true;
                    return true;
                }
            }

            saved.Restore();
            return false;
        }
    }
}
