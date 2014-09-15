using CommonMarkSharp.Inlines;

namespace CommonMarkSharp.InlineParsers
{
    public class BacktickParser : IInlineParser<InlineString>
    {
        public string StartsWithChars { get { return "`"; } }

        public bool CanParse(Subject subject)
        {
            return subject.Char == '`';
        }

        public InlineString Parse(ParserContext context, Subject subject)
        {
            if (!CanParse(subject)) return null;

            return new InlineString(subject.TakeWhile('`'));
        }
    }
}
