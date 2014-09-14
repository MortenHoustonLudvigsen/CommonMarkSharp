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
    public class RawHtmlParser : IParser<RawHtml>
    {
        public string StartsWithChars
        {
            get { return "<"; }
        }

        public bool CanParse(Subject subject)
        {
            return subject.Char == '<';
        }

        //    // htmltag = opentag | closetag | htmlcomment | processinginstruction |
        //    //           declaration | cdata;

        public RawHtml Parse(ParserContext context, Subject subject)
        {
            if (!this.CanParse(subject)) return null;

            var savedSubject = subject.Save();

            var matched = MatchOpenTag(subject);
            matched = matched || MatchCloseTag(subject);
            matched = matched || MatchHtmlComment(subject);
            matched = matched || MatchProcessingInstruction(subject);
            matched = matched || MatchDeclaration(subject);
            matched = matched || MatchCData(subject);

            if (matched)
            {
                return new RawHtml(savedSubject.GetLiteral());
            }

            savedSubject.Restore();
            return null;
        }

        // opentag = [<] tagname attribute* spacechar* [/]? [>];
        private bool MatchOpenTag(Subject subject)
        {
            if (subject.Char != '<')
            {
                return false;
            }
            var savedSubject = subject.Save();
            subject.Advance();
            if (MatchTagName(subject))
            {
                while (MatchAttribute(subject))
                {
                }
                subject.SkipWhiteSpace();
                if (subject.Char == '/')
                {
                    subject.Advance();
                }
                if (subject.Char == '>')
                {
                    subject.Advance();
                    return true;
                }
            }
            savedSubject.Restore();
            return false;
        }

        // closetag = [<] [/] tagname spacechar* [>];
        private bool MatchCloseTag(Subject subject)
        {
            if (!subject.StartsWith("</"))
            {
                return false;
            }
            var savedSubject = subject.Save();
            subject.Advance(2);
            if (MatchTagName(subject))
            {
                subject.SkipWhiteSpace();
                if (subject.Char == '>')
                {
                    subject.Advance();
                    return true;
                }
            }
            savedSubject.Restore();
            return false;
        }

        // htmlcomment = "<!--" ([^-\x00]+ | [-][^-\x00]+)* "-->";
        private bool MatchHtmlComment(Subject subject)
        {
            if (!subject.StartsWith("<!--"))
            {
                return false;
            }
            var savedSubject = subject.Save();
            subject.Advance(4);

            var finished = false;
            do
            {
                if (subject.Char == '-' && subject[1] != '-')
                {
                    subject.Advance();
                }
                else
                {
                    finished = subject.AdvanceWhile(c => c != '-') == 0;
                }
            } while (!finished);

            if (subject.StartsWith("-->"))
            {
                subject.Advance(3);
                return true;
            }

            savedSubject.Restore();
            return false;
        }

        // processinginstruction = "<?" ([^?>\x00]+ | [?][^>\x00])* "?>";
        private bool MatchProcessingInstruction(Subject subject)
        {
            if (!subject.StartsWith("<?"))
            {
                return false;
            }
            var savedSubject = subject.Save();
            subject.Advance(2);

            var finished = false;
            do
            {
                if (subject.Char == '?' && subject[1] != '>')
                {
                    subject.Advance();
                }
                else
                {
                    finished = subject.AdvanceWhile(c => c != '?' && c != '>') == 0;
                }
            } while (!finished);

            if (subject.StartsWith("?>"))
            {
                subject.Advance(2);
                return true;
            }

            savedSubject.Restore();
            return false;
        }

        // declaration = "<!" [A-Z]+ spacechar+ [^>\x00]* ">";
        private static readonly HashSet<char> _declarationChars = new HashSet<char>(Patterns.UpperCaseAlphas);
        private bool MatchDeclaration(Subject subject)
        {
            if (!subject.StartsWith("<!"))
            {
                return false;
            }
            var savedSubject = subject.Save();
            subject.Advance(2);

            if (subject.AdvanceWhile(c => _declarationChars.Contains(c)) > 0)
            {
                subject.SkipWhiteSpace();
                subject.AdvanceWhile(c => c != '>');

                if (subject.Char == '>')
                {
                    subject.Advance();
                    return true;
                }
            }

            savedSubject.Restore();
            return false;
        }

        // cdata = "<![CDATA[" ([^\]\x00]+ | "]" [^\]\x00] | "]]" [^>\x00])* "]]>";
        private bool MatchCData(Subject subject)
        {
            if (!subject.StartsWith("<![CDATA["))
            {
                return false;
            }
            var savedSubject = subject.Save();
            subject.Advance(8);

            var finished = false;
            do
            {
                if (subject.StartsWith("]]") && subject[2] != '>')
                {
                    subject.Advance(2);
                }
                else if (subject.Char == ']' && subject[1] != ']')
                {
                    subject.Advance(1);
                }
                else
                {
                    finished = subject.AdvanceWhile(c => c != ']') == 0;
                }
            } while (!finished);


            if (subject.StartsWith("]]>"))
            {
                subject.Advance(3);
                return true;
            }

            savedSubject.Restore();
            return false;
        }

        // attribute = spacechar+ attributename attributevaluespec?;
        private bool MatchAttribute(Subject subject)
        {
            if (!subject.IsWhiteSpace())
            {
                return false;
            }

            var savedSubject = subject.Save();
            subject.Advance();
            if (MatchAttributeName(subject))
            {
                MatchAttributeValueSpec(subject);
                return true;
            }

            savedSubject.Restore();
            return false;
        }

        // attributevaluespec = spacechar* [=] spacechar* attributevalue;
        private bool MatchAttributeValueSpec(Subject subject)
        {
            var savedSubject = subject.Save();
            subject.SkipWhiteSpace();
            if (subject.Char != '=')
            {
                savedSubject.Restore();
                return false;
            }
            subject.Advance();
            subject.SkipWhiteSpace();

            if (MatchAttributeValue(subject))
            {
                return true;
            }

            savedSubject.Restore();
            return false;
        }

        // attributevalue = unquotedvalue | singlequotedvalue | doublequotedvalue;
        private bool MatchAttributeValue(Subject subject)
        {
            return MatchUnquotedValue(subject) || MatchDoubleQuotedValue(subject) || MatchSingleQuotedValue(subject);
        }

        // unquotedvalue = [^\"'=<>`\x00]+;
        private static readonly HashSet<char> _unquotedValueChars = new HashSet<char>("\"'=<>`");
        private bool MatchUnquotedValue(Subject subject)
        {
            if (subject.AdvanceWhile(c => !_unquotedValueChars.Contains(c)) > 0)
            {
                return true;
            }
            return false;
        }

        // singlequotedvalue = ['][^'\x00]*['];
        private bool MatchSingleQuotedValue(Subject subject)
        {
            if (subject.Char != '\'')
            {
                return false;
            }
            var savedSubject = subject.Save();
            subject.Advance();
            subject.AdvanceWhile(c => c != '\'');
            if (subject.Char == '\'')
            {
                subject.Advance();
                return true;
            }
            savedSubject.Restore();
            return false;
        }

        // doublequotedvalue = [\"][^\"\x00]*[\"];
        private bool MatchDoubleQuotedValue(Subject subject)
        {
            if (subject.Char != '"')
            {
                return false;
            }
            var savedSubject = subject.Save();
            subject.Advance();
            subject.AdvanceWhile(c => c != '"');
            if (subject.Char == '"')
            {
                subject.Advance();
                return true;
            }
            savedSubject.Restore();
            return false;
        }

        // attributename = [a-zA-Z_:][a-zA-Z0-9:._-]*;
        private static readonly HashSet<char> _attributeNameStartChars = new HashSet<char>(Patterns.Alphas + "_:");
        private static readonly HashSet<char> _attributeNameChars = new HashSet<char>(Patterns.Alphanums + ":._-");
        private bool MatchAttributeName(Subject subject)
        {
            if (_attributeNameStartChars.Contains(subject.Char))
            {
                subject.AdvanceWhile(c => _attributeNameChars.Contains(c));
                return true;
            }
            return false;
        }

        // tagname = [A-Za-z][A-Za-z0-9]*;
        private static readonly HashSet<char> _tagNameStartChars = new HashSet<char>(Patterns.Alphas);
        private static readonly HashSet<char> _tagNameChars = new HashSet<char>(Patterns.Alphanums);
        private bool MatchTagName(Subject subject)
        {
            if (_tagNameStartChars.Contains(subject.Char))
            {
                subject.AdvanceWhile(c => _tagNameChars.Contains(c));
                return true;
            }
            return false;
        }
    }

    //public class RawHtmlParser : RegexParser<RawHtml>
    //{
    //    // tagname = [A-Za-z][A-Za-z0-9]*;
    //    public static readonly string TagName = @"[A-Za-z][A-Za-z0-9]*";

    //    // attributename = [a-zA-Z_:][a-zA-Z0-9:._-]*;
    //    public static readonly string AttributeName = @"[a-zA-Z_:][a-zA-Z0-9:._-]*";

    //    // unquotedvalue = [^\"'=<>`\x00]+;
    //    public static readonly string UnquotedValue = @"[^\""'=<>`\x00]+";
    //    // singlequotedvalue = ['][^'\x00]*['];
    //    public static readonly string SingleQuotedValue = @"['][^'\x00]*[']";
    //    // doublequotedvalue = [\"][^\"\x00]*[\"];
    //    public static readonly string DoubleQuotedValue = @"[\""][^\""\x00]*[\""]";

    //    // attributevalue = unquotedvalue | singlequotedvalue | doublequotedvalue;
    //    public static readonly string AttributeValue = RegexUtils.Join(UnquotedValue, SingleQuotedValue, DoubleQuotedValue);

    //    // attributevaluespec = spacechar* [=] spacechar* attributevalue;
    //    public static readonly string AttributeValueSpec = string.Format(@"{0}*={0}*{1}", Patterns.SpaceChar, AttributeValue);

    //    // attribute = spacechar+ attributename attributevaluespec?;
    //    public static readonly string Attribute = string.Format(@"{0}+{1}(?:{2})?", Patterns.SpaceChar, AttributeName, AttributeValueSpec);

    //    // opentag = tagname attribute* spacechar* [/]? [>];
    //    public static readonly string OpenTag = string.Format(@"<{0}(?:{1})*{2}*/?>", TagName, Attribute, Patterns.SpaceChar);

    //    // closetag = [/] tagname spacechar* [>];
    //    public static readonly string CloseTag = string.Format(@"</{0}{1}*>", TagName, Patterns.SpaceChar);

    //    // htmlcomment = "!--" ([^-\x00]+ | [-][^-\x00]+)* "-->";
    //    public static readonly string HtmlComment = @"<!--(?:[^-\x00]+|[-][^-\x00]+)*-->";

    //    // processinginstruction = "?" ([^?>\x00]+ | [?][^>\x00])* "?>";
    //    public static readonly string ProcessingInstruction = @"<\?(?:[^?>\x00]+|\?[^>\x00])*\?>";

    //    // declaration = "!" [A-Z]+ spacechar+ [^>\x00]* ">";
    //    public static readonly string Declaration = string.Format(@"<![A-Z]+{0}+[^>\x00]*>", Patterns.SpaceChar);

    //    // cdata = "![CDATA[" ([^\]\x00]+ | "]" [^\]\x00] | "]]" [^>\x00])* "]]>";
    //    public static readonly string CData = @"<!\[CDATA\[(?:[^\]\x00]+|\][^\]\x00]|\]\][^>\x00])*\]\]>";

    //    // htmltag = opentag | closetag | htmlcomment | processinginstruction |
    //    //           declaration | cdata;
    //    public static readonly string HtmlTag = RegexUtils.Join(OpenTag, CloseTag, HtmlComment, ProcessingInstruction, Declaration, CData);

    //    // Try to match an HTML tag after first <.
    //    public static readonly Regex HtmlTagRe = RegexUtils.Create(@"\G{0}", HtmlTag);

    //    public RawHtmlParser()
    //        : base(HtmlTagRe)
    //    {
    //        StartsWithChars = "<";
    //    }

    //    protected override RawHtml HandleMatch(ParserContext context, string[] groups)
    //    {
    //        return new RawHtml(groups[0]);
    //    }
    //}
}
