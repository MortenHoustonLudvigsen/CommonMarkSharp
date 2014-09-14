using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Blocks
{
    public class HtmlBlock : LeafBlock
    {
        public override bool AcceptsLines { get { return true; } }

        public override void Close(CommonMarkParser parser, int lineNumber)
        {
            base.Close(parser, lineNumber);
            Contents = string.Join("\n", Strings);
        }

        public override bool MatchNextLine(Subject subject)
        {
            return !subject.IsBlank;
        }
    }
}
