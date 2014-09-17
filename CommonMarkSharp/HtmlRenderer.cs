using CommonMarkSharp.Blocks;
using CommonMarkSharp.Inlines;
using System.Collections.Generic;
using System.Linq;

namespace CommonMarkSharp
{
    public class HtmlRenderer : MarkupRenderer
    {
        public virtual void Visit(Document document)
        {
            Write(document.Children);
        }

        public virtual void Visit(HorizontalRule horizontalRule)
        {
            WriteClosedTag(horizontalRule, "hr");
            WriteLine();
        }

        public virtual void Visit(Header header)
        {
            var tag = string.Format("h{0}", header.Level);
            WriteStartTag(header, tag);
            Write(header.Inlines);
            WriteEndTag(header, tag);
            WriteLine();
        }

        public virtual void Visit(Paragraph paragraph)
        {
            WriteStartTag(paragraph, "p");
            Write(paragraph.Inlines);
            WriteEndTag(paragraph, "p");
            WriteLine();
        }

        public virtual void Visit(BlockQuote quote)
        {
            WriteStartTag(quote, "blockquote");
            WriteLine(true);
            Write(quote.Children);
            WriteEndTag(quote, "blockquote");
            WriteLine();
        }

        public virtual void Visit(IndentedCode code)
        {
            WriteStartTag(code, "pre");
            WriteStartTag(code, "code");
            WriteEscaped(code.Contents);
            WriteEndTag(code, "code");
            WriteEndTag(code, "pre");
            WriteLine();
        }

        public virtual void Visit(FencedCode code)
        {
            var language = code.InfoWords.FirstOrDefault();
            var attributes = new List<Attribute>();
            if (!string.IsNullOrEmpty(language))
            {
                attributes.Add(new Attribute("class", "language-" + language));
            }
            WriteStartTag(code, "pre");
            WriteStartTag(code, "code", attributes);
            WriteEscaped(code.Contents);
            WriteEndTag(code, "code");
            WriteEndTag(code, "pre");
            WriteLine();
        }

        private Stack<bool> _inTightList = new Stack<bool>(new[] { false });

        public virtual void Visit(List list)
        {
            _inTightList.Push(list.Tight);
            var tag = list.Data.Type == "Bullet" ? "ul" : "ol";
            var attributes = new List<Attribute>();
            if (list.Data.Start != null && list.Data.Start != 1)
            {
                attributes.Add(new Attribute("start", list.Data.Start.ToString()));
            }
            WriteStartTag(list, tag, attributes);
            Write(list.Children);
            WriteEndTag(list, tag);
            if (!(list.Parent is ListItem))
            {
                WriteLine();
            }
            _inTightList.Pop();
        }

        public virtual void Visit(ListItem item)
        {
            var inTightList = _inTightList.Peek();
            var tag = item.Data.Type == "Bullet" ? "ul" : "ol";
            WriteLine();
            WriteStartTag(item, "li");
            foreach (var part in item.Children)
            {
                var isLast = part == item.LastChild;
                var paragraph = part as Paragraph;
                if (paragraph != null && (inTightList || !paragraph.Inlines.Any()))
                {
                    Write(paragraph.Inlines);
                }
                else
                {
                    part.Accept(this);
                }
                if (!isLast)
                {
                    WriteLine();
                }
            }
            WriteEndTag(item, "li");
            WriteLine();
        }

        public virtual void Visit(HtmlBlock html)
        {
            Write(html.Contents);
            WriteLine();
        }

        public virtual void Visit(InlineString inline)
        {
            WriteEscaped(inline.Value);
        }

        public virtual void Visit(HardBreak inline)
        {
            WriteClosedTag(inline, "br");
            WriteLine();
        }

        public virtual void Visit(SoftBreak inline)
        {
            WriteLine();
        }

        public virtual void Visit(InlineList list)
        {
            Write(list.Inlines);
        }

        public virtual void Visit(Entity inline)
        {
            Write(inline.Value);
        }

        public virtual void Visit(Emphasis inline)
        {
            WriteStartTag(inline, "em");
            Write(inline.Inlines);
            WriteEndTag(inline, "em");
        }

        public virtual void Visit(StrongEmphasis inline)
        {
            WriteStartTag(inline, "strong");
            Write(inline.Inlines);
            WriteEndTag(inline, "strong");
        }

        public virtual void Visit(Link inline)
        {
            WriteStartTag(inline, "a",
                new Attribute("href", inline.Destination.Inlines, true),
                new Attribute("title", inline.Title.Inlines)
            );
            Write(inline.Label.Inlines);
            WriteEndTag(inline, "a");
        }

        public virtual void Visit(LinkLabel inline)
        {
            Write(inline.Inlines);
        }

        public virtual void Visit(LinkReference inline)
        {
            var label = inline.Label == null ? inline.Link.Label : inline.Label;
            WriteStartTag(inline, "a",
                new Attribute("href", inline.Link.Destination.Inlines, true),
                new Attribute("title", inline.Link.Title.Inlines)
            );
            Write(label.Inlines);
            WriteEndTag(inline, "a");
        }

        public virtual void Visit(Image inline)
        {
            WriteClosedTag(inline, "img",
                new Attribute("src", inline.Link.Destination.Inlines, true),
                new Attribute("alt", inline.Link.Label.Inlines, true),
                new Attribute("title", inline.Link.Title.Inlines)
            );
        }

        public virtual void Visit(ImageReference inline)
        {
            var label = inline.LinkReference.Label == null ? inline.LinkReference.Link.Label : inline.LinkReference.Label;
            WriteClosedTag(inline, "img",
                new Attribute("src", inline.LinkReference.Link.Destination.Inlines, true),
                new Attribute("alt", label.Inlines, true),
                new Attribute("title", inline.LinkReference.Link.Title.Inlines)
            );
        }

        public virtual void Visit(InlineCode inline)
        {
            WriteStartTag(inline, "code");
            WriteEscaped(inline.Code);
            WriteEndTag(inline, "code");
        }

        public virtual void Visit(RawHtml inline)
        {
            Write(inline.Value);
        }

        protected virtual void WriteLine(bool force)
        {
            if (force)
            {
                base.WriteLine();
            }
            else
            {
                WriteLine();
            }
        }

        protected override void WriteLine()
        {
            var shouldWrite = true;
            var block = Current as Block;
            if (block != null && block.Parent is ListItem)
            {
                shouldWrite = false;
            }
            if (shouldWrite)
            {
                base.WriteLine();
            }
        }
    }
}
