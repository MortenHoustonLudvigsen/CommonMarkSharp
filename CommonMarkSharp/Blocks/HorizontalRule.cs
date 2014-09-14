namespace CommonMarkSharp.Blocks
{
    public class HorizontalRule : LeafBlock
    {
        public override bool MatchNextLine(Subject subject)
        {
            return false;
        }
    }
}
