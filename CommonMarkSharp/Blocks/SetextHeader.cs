namespace CommonMarkSharp.Blocks
{
    public class SetExtHeader : Header
    {
        public SetExtHeader(int level, string contents)
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
