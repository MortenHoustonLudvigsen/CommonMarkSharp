using CommonMarkSharp.Blocks;
using CommonMarkSharp.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            WriteLine("<hr />");
        }

        public virtual void Visit(Header header)
        {
            Write("<h{0}>", header.Level);
            Write(header.Inlines);
            Write("</h{0}>", header.Level);
            WriteLine();
        }

        public virtual void Visit(Paragraph paragraph)
        {
            Write("<p>");
            Write(paragraph.Inlines);
            WriteLine("</p>");
        }

        public virtual void Visit(BlockQuote quote)
        {
            Write("<blockquote>");
            WriteLine(true);
            Write(quote.Children);
            WriteLine("</blockquote>");
        }

        public virtual void Visit(IndentedCode code)
        {
            Write("<pre>");
            Write("<code>");
            Write(Escape(code.Contents));
            Write("</code>");
            Write("</pre>");
            WriteLine();
        }

        public virtual void Visit(FencedCode code)
        {
            var language = code.InfoWords.FirstOrDefault();
            Write("<pre>");
            Write("<code");
            if (!string.IsNullOrEmpty(language))
            {
                WriteAttribute("class", Escape("language-" + language, true));
            }
            Write(">");
            Write(Escape(code.Contents));
            Write("</code>");
            Write("</pre>");
            WriteLine();
        }

        private Stack<bool> _inTightList = new Stack<bool>(new[] { false });

        public virtual void Visit(List list)
        {
            _inTightList.Push(list.Tight);
            var tag = list.Data.Type == "Bullet" ? "ul" : "ol";
            Write("<{0}", tag);
            if (list.Data.Start != null && list.Data.Start != 1)
            {
                WriteAttribute("start", list.Data.Start.ToString());
            }
            WriteLine(">");
            Write(list.Children);
            Write("</{0}>", tag);
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
            Write("<li>");
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
            WriteLine("</li>");
        }

        public virtual void Visit(HtmlBlock html)
        {
            Write(html.Contents);
            WriteLine();
        }

        public virtual void Visit(InlineString inline)
        {
            Write(Escape(inline.Value));
        }

        public virtual void Visit(HardBreak inline)
        {
            WriteLine("<br />");
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
            Write(EscapeInAttribute("<em>"));
            Write(inline.Inlines);
            Write(EscapeInAttribute("</em>"));
        }

        public virtual void Visit(StrongEmphasis inline)
        {
            Write("<strong>");
            Write(inline.Inlines);
            Write("</strong>");
        }

        public virtual void Visit(Link inline)
        {
            Write("<a");
            WriteAttribute("href", inline.Destination.Inlines, true);
            WriteAttribute("title", inline.Title.Inlines);
            Write(">");
            Write(inline.Label.Inlines);
            Write("</a>");
        }

        public virtual void Visit(LinkLabel inline)
        {
            Write(inline.Inlines);
        }

        public virtual void Visit(LinkReference inline)
        {
            Write("<a");
            WriteAttribute("href", inline.Link.Destination.Inlines, true);
            WriteAttribute("title", inline.Link.Title.Inlines);
            Write(">");
            if (inline.Label == null)
            {
                Write(inline.Link.Label.Inlines);
            }
            else
            {
                Write(inline.Label.Inlines);
            }
            Write("</a>");
        }

        public virtual void Visit(Image inline)
        {
            Write("<img");
            WriteAttribute("src", inline.Link.Destination.Inlines, true);
            WriteAttribute("alt", inline.Link.Label.Inlines, true);
            WriteAttribute("title", inline.Link.Title.Inlines);
            Write(" />");
        }

        public virtual void Visit(ImageReference inline)
        {
            Write("<img");
            WriteAttribute("src", inline.LinkReference.Link.Destination.Inlines, true);
            if (inline.LinkReference.Label == null)
            {
                WriteAttribute("alt", inline.LinkReference.Link.Label.Inlines, true);
            }
            else
            {
                WriteAttribute("alt", inline.LinkReference.Label.Inlines, true);
            }
            WriteAttribute("title", inline.LinkReference.Link.Title.Inlines);
            Write(" />");
        }

        public virtual void Visit(InlineCode inline)
        {
            Write("<code>{0}</code>", Escape(inline.Code));
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
