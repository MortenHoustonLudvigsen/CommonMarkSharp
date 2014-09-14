namespace CommonMarkSharp.Blocks
{
    public class ListItem : ContainerBlock
    {
        public ListData Data { get; set; }

        public override bool CanContain(Block block)
        {
            return true;
        }

        // Returns true if block ends with a blank line, descending if needed
        // into lists and sublists.
        public override bool EndsWithBlankLine
        {
            get
            {
                if (LastLineIsBlank)
                {
                    return true;
                }
                if (LastChild != null)
                {
                    return LastChild.EndsWithBlankLine;
                }
                return false;
            }
        }

        public override bool MatchNextLine(Subject subject)
        {
            if (subject.Indent >= Data.MarkerOffset + Data.Padding)
            {
                subject.Advance(Data.MarkerOffset + Data.Padding);
            }
            else if (subject.IsBlank)
            {
                subject.AdvanceToFirstNonSpace();
            }
            else
            {
                return false;
            }
            return true;
        }
    }
}
