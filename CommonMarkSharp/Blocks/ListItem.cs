using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public override bool MatchNextLine(LineInfo lineInfo)
        {
            if (lineInfo.Indent >= Data.MarkerOffset + Data.Padding)
            {
                lineInfo.Offset += Data.MarkerOffset + Data.Padding;
            }
            else if (lineInfo.Blank)
            {
                lineInfo.Offset = lineInfo.FirstNonSpace;
            }
            else
            {
                return false;
            }
            return true;
        }
    }
}
