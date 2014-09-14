using CommonMarkSharp.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonMarkSharp.BlockParsers
{
    public class HtmlBlockParser : IBlockParser<HtmlBlock>
    {
        private static readonly HashSet<string> _tagNames = new HashSet<string>(new[]
        {
            "article", "header", "aside", "hgroup", "iframe", "blockquote", "hr", "body",
            "li", "map", "button", "object", "canvas", "ol", "caption", "output", "col",
            "p", "colgroup", "pre", "dd", "progress", "div", "section", "dl", "table",
            "td", "dt", "tbody", "embed", "textarea", "fieldset", "tfoot", "figcaption",
            "th", "figure", "thead", "footer", "footer", "tr", "form", "ul", "h1", "h2",
            "h3", "h4", "h5", "h6", "video", "script", "style"
        }, StringComparer.OrdinalIgnoreCase);

        private static readonly int _maxTagNameLength = _tagNames.Select(t => t.Length).Max();

        private static readonly HashSet<char> _tagNameChars = new HashSet<char>(
            _tagNames.SelectMany(t => t).Distinct().Concat(_tagNames.SelectMany(t => t.ToUpper()).Distinct())
        );

        public bool Parse(ParserContext context, Subject subject)
        {
            var saved = subject.Save();

            subject.AdvanceWhile(c => c == ' ', 3);

            if (subject.Char == '<')
            {
                subject.Advance();
                if (subject.Char == '!' || subject.Char == '?')
                {
                    subject.Advance();
                }
                else
                {
                    if (subject.Char == '/')
                    {
                        subject.Advance();

                        var tagName = subject.TakeWhile(c => _tagNameChars.Contains(c), _maxTagNameLength);
                        if ((subject.Char != ' ' && subject.Char != '>') || !_tagNames.Contains(tagName))
                        {
                            saved.Restore();
                            return false;
                        }
                    }
                    else
                    {
                        var tagName = subject.TakeWhile(c => _tagNameChars.Contains(c), _maxTagNameLength);
                        if ((subject.Char != ' ' && subject.Char != '>' && !subject.StartsWith("/>")) || !_tagNames.Contains(tagName))
                        {
                            saved.Restore();
                            return false;
                        }
                    }
                }

                context.AddBlock(new HtmlBlock());
                context.BlocksParsed = true;
                // note, we restore subject because the tag is part of the text
                saved.Restore();
                return true;
            }

            saved.Restore();
            return false;
        }
    }
}
