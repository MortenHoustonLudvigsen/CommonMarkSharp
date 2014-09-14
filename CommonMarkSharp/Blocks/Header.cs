namespace CommonMarkSharp.Blocks
{
    public class Header : LeafBlockWithInlines
    {
        public Header(int level, string contents)
        {
            Level = level;
            AddLine(contents);
        }

        public int Level { get; set; }
        public override bool AcceptsLines { get { return true; } }

        public override bool MatchNextLine(Subject subject)
        {
            return false;
        }
    }
}
