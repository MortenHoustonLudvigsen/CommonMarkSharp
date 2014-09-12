using CommonMarkSharp.Parsing.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonMarkSharp.Parsing.Blocks
{
    public class FencedCode : LeafBlock
    {
        private static Regex _spaceRe = new Regex(" +", RegexOptions.Compiled);

        public string Info { get; private set; }
        public int Length { get; set; }
        public char Char { get; set; }
        public int Offset { get; set; }
        public IEnumerable<string> InfoWords { get { return _spaceRe.Split(Info); } }

        public override bool AcceptsLines { get { return true; } }

        public override void Close(CommonMarkParser parser, int lineNumber)
        {
            base.Close(parser, lineNumber);
            Info = string.Join("", parser.Parsers.EscapedStringParser.ParseMany(Document, Strings.First()).Cast<InlineString>().Select(s => s.Value)).Trim();
            Contents = Strings.Count() > 1 ? string.Join("\n", Strings.Skip(1)) + "\n" : "";
        }

        public override bool MatchNextLine(LineInfo lineInfo)
        {
            // skip optional spaces of fence offset
            var i = Offset;
            while (i > 0 && lineInfo[lineInfo.Offset] == ' ')
            {
                lineInfo.Offset++;
                i--;
            }
            return true;
        }
    }
}
