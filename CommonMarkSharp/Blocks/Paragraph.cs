using System.Text.RegularExpressions;

namespace CommonMarkSharp.Blocks
{
    public class Paragraph : LeafBlockWithInlines
    {
        private static readonly Regex LeadingSpaceRe = new Regex(@"^ +", RegexOptions.Compiled);

        public override bool AcceptsLines
        {
            get { return true; }
        }

        public override void Close(ParserContext context)
        {
            base.Close(context);
            var subject = new Subject(string.Join("\n", Strings));
            subject.SkipWhiteSpace();
            var hasLinkDefinition = false;
            var linkDefinition = context.Parsers.LinkDefinitionParser.Parse(context, subject);
            while (linkDefinition != null)
            {
                hasLinkDefinition = true;
                Document.AddLinkDefinition(new LinkDefinition(linkDefinition));
                linkDefinition = context.Parsers.LinkDefinitionParser.Parse(context, subject);
            }
            subject.SkipWhiteSpace();
            if (hasLinkDefinition && subject.EndOfString)
            {
                Parent.Remove(this);
            }
            Contents = subject.Rest;
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
