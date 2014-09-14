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

        public override void Close(ParserContext context)
        {
            base.Close(context);
            Contents = LeadingSpaceRe.Replace(string.Join("\n", Strings), "");
            var subject = new Subject(Contents);
            var hasLinkDefinition = false;
            var linkDefinition = context.Parsers.LinkDefinitionParser.Parse(context, subject);
            while (linkDefinition != null)
            {
                hasLinkDefinition = true;
                Document.AddLinkDefinition(new LinkDefinition(linkDefinition));
                linkDefinition = context.Parsers.LinkDefinitionParser.Parse(context, subject);
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

        public override bool MatchNextLine(Subject subject)
        {
            if (subject.IsBlank)
            {
                LastLineIsBlank = true;
                return false;
            }
            return true;
        }
    }
}
