using CommonMarkSharp.Blocks;
using CommonMarkSharp.Inlines;
using CommonMarkSharp.InlineParsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp
{
    public class HandleInlinesVisitor : CommonMarkVisitor
    {
        public HandleInlinesVisitor(ParserContext context)
        {
            Context = context;
        }

        public ParserContext Context { get; private set; }

        private IEnumerable<Inline> ParseInlines(Block block)
        {
            return Context.Parsers.CommonMarkInlineParser.ParseMany(Context, block.Contents.Trim());
        }

        public virtual void Visit(Block document)
        {
            document.Children.Accept(this);
        }

        public virtual void Visit(Paragraph paragraph)
        {
            paragraph.Inlines = ParseInlines(paragraph);
        }

        public virtual void Visit(Header header)
        {
            header.Inlines = ParseInlines(header);
        }
    }
}
