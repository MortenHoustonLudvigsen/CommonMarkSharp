using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonMarkSharp.InlineParsers;
using System.Text.RegularExpressions;
using CommonMarkSharp.Inlines;

namespace CommonMarkSharp.Blocks
{
    public class Paragraph : LeafBlock
    {
        private static readonly Regex LeadingSpaceRe = new Regex(@"^ +", RegexOptions.Compiled);

        public IEnumerable<Inline> Inlines { get; set; }

        public override bool AcceptsLines
        {
            get { return true; }
        }

        public override void Close(CommonMarkParser parser, int lineNumber)
        {
            base.Close(parser, lineNumber);
            Contents = LeadingSpaceRe.Replace(string.Join("\n", Strings), "");
            var subject = new Subject(Contents);
            var hasLinkDefinition = false;
            var linkDefinition = parser.Parsers.LinkDefinitionParser.Parse(this.Document, subject);
            while (linkDefinition != null)
            {
                hasLinkDefinition = true;
                Document.AddLinkDefinition(new LinkDefinition(linkDefinition));
                linkDefinition = parser.Parsers.LinkDefinitionParser.Parse(this.Document, subject);
            }
            if (hasLinkDefinition)
            {
                Contents = subject.Rest;
                if (string.IsNullOrWhiteSpace(Contents))
                {
                    Parent.Remove(this);
                }
            }
        }

        public override bool MatchNextLine(LineInfo lineInfo)
        {
            if (lineInfo.Blank)
            {
                LastLineIsBlank = true;
                return false;
            }
            return true;
        }
    }
}
