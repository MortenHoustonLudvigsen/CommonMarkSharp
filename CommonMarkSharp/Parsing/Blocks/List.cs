using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Parsing.Blocks
{
    public class List : ContainerBlock
    {
        public ListData Data { get; set; }
        public bool Tight { get; private set; }

        public override bool CanContain(Block block)
        {
            return block is ListItem;
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


        public override void Close(CommonMarkParser parser, int lineNumber)
        {
            base.Close(parser, lineNumber);

            Tight = true; // tight by default

            foreach (var item in Children)
            {
                // check for non-final list item ending with blank line:
                if (item.EndsWithBlankLine && item != LastChild)
                {
                    Tight = false;
                    break;
                }

                // recurse into children of list item, to see if there are
                // spaces between any of them:
                foreach (var subItem in item.Children)
                {
                    if (subItem.EndsWithBlankLine && (item != LastChild || subItem != item.LastChild))
                    {
                        Tight = false;
                        break;
                    }
                }
            }
        }
    }
}
