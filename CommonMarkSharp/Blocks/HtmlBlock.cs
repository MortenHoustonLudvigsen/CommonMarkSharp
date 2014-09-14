namespace CommonMarkSharp.Blocks
{
    public class HtmlBlock : LeafBlock
    {
        public override bool AcceptsLines { get { return true; } }
        public override bool IsCode { get { return true; } }

        public override void Close(ParserContext context)
        {
            base.Close(context);
            Contents = string.Join("\n", Strings);
        }

        public override bool MatchNextLine(Subject subject)
        {
            return !subject.IsBlank;
        }
    }
}
