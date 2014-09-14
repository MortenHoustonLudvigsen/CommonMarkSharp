namespace CommonMarkSharp.Blocks
{
    public class ATXHeader : Header
    {
        public ATXHeader(int level, string contents)
            : base(level, contents)
        {
        }

        public override void Close(ParserContext context)
        {
            base.Close(context);
            Contents = string.Join("\n", Strings);
        }
    }
}
