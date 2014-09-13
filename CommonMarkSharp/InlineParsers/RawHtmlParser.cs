using CommonMarkSharp.Blocks;
using CommonMarkSharp.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonMarkSharp.InlineParsers
{
    public class RawHtmlParser : RegexParser<InlineRawHtml>
    {
        // tagname = [A-Za-z][A-Za-z0-9]*;
        public static readonly string TagName = @"[A-Za-z][A-Za-z0-9]*";

        // attributename = [a-zA-Z_:][a-zA-Z0-9:._-]*;
        public static readonly string AttributeName = @"[a-zA-Z_:][a-zA-Z0-9:._-]*";

        // unquotedvalue = [^\"'=<>`\x00]+;
        public static readonly string UnquotedValue = @"[^\""'=<>`\x00]+";
        // singlequotedvalue = ['][^'\x00]*['];
        public static readonly string SingleQuotedValue = @"['][^'\x00]*[']";
        // doublequotedvalue = [\"][^\"\x00]*[\"];
        public static readonly string DoubleQuotedValue = @"[\""][^\""\x00]*[\""]";

        // attributevalue = unquotedvalue | singlequotedvalue | doublequotedvalue;
        public static readonly string AttributeValue = RegexUtils.Join(UnquotedValue, SingleQuotedValue, DoubleQuotedValue);

        // attributevaluespec = spacechar* [=] spacechar* attributevalue;
        public static readonly string AttributeValueSpec = string.Format(@"{0}*={0}*{1}", Patterns.SpaceChar, AttributeValue);

        // attribute = spacechar+ attributename attributevaluespec?;
        public static readonly string Attribute = string.Format(@"{0}+{1}(?:{2})?", Patterns.SpaceChar, AttributeName, AttributeValueSpec);

        // opentag = tagname attribute* spacechar* [/]? [>];
        public static readonly string OpenTag = string.Format(@"<{0}(?:{1})*{2}*/?>", TagName, Attribute, Patterns.SpaceChar);

        // closetag = [/] tagname spacechar* [>];
        public static readonly string CloseTag = string.Format(@"</{0}{1}*>", TagName, Patterns.SpaceChar);

        // htmlcomment = "!--" ([^-\x00]+ | [-][^-\x00]+)* "-->";
        public static readonly string HtmlComment = @"<!--(?:[^-\x00]+|[-][^-\x00]+)*-->";

        // processinginstruction = "?" ([^?>\x00]+ | [?][^>\x00])* "?>";
        public static readonly string ProcessingInstruction = @"<\?(?:[^?>\x00]+|\?[^>\x00])*\?>";

        // declaration = "!" [A-Z]+ spacechar+ [^>\x00]* ">";
        public static readonly string Declaration = string.Format(@"<![A-Z]+{0}+[^>\x00]*>", Patterns.SpaceChar);

        // cdata = "![CDATA[" ([^\]\x00]+ | "]" [^\]\x00] | "]]" [^>\x00])* "]]>";
        public static readonly string CData = @"<!\[CDATA\[(?:[^\]\x00]+|\][^\]\x00]|\]\][^>\x00])*\]\]>";

        // htmltag = opentag | closetag | htmlcomment | processinginstruction |
        //           declaration | cdata;
        public static readonly string HtmlTag = RegexUtils.Join(OpenTag, CloseTag, HtmlComment, ProcessingInstruction, Declaration, CData);

        // Try to match an HTML tag after first <.
        public static readonly Regex HtmlTagRe = RegexUtils.Create(@"\G{0}", HtmlTag);

        public RawHtmlParser()
            : base(HtmlTagRe)
        {
            StartsWithChars = "<";
        }

        protected override InlineRawHtml HandleMatch(ParserContext context, string[] groups)
        {
            return new InlineRawHtml(groups[0]);
        }
    }
}
