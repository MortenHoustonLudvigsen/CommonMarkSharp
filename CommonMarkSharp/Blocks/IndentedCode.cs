using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonMarkSharp.Blocks
{
    public class IndentedCode : LeafBlock
    {
        private static readonly Regex TrailingEmptyLinesRe = new Regex(@"(?:\n *)+$", RegexOptions.Compiled);

        public override bool AcceptsLines { get { return true; } }

        public override void Close(CommonMarkParser parser, int lineNumber)
        {
            base.Close(parser, lineNumber);
            Contents = TrailingEmptyLinesRe.Replace(string.Join("\n", Strings), "") + "\n";
        }

        public override bool MatchNextLine(LineInfo lineInfo)
        {
            if (lineInfo.Indent >= CommonMarkParser.CODE_INDENT)
            {
                lineInfo.Offset += CommonMarkParser.CODE_INDENT;
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
