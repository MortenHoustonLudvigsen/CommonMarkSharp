using CommonMarkSharp.Inlines;
using CommonMarkSharp.InlineParsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonMarkSharp.Blocks;
using CommonMarkSharp.BlockParsers;

namespace CommonMarkSharp
{
    public class Parsers
    {
        public Parsers()
        {
            // Create inline parsers
            LineBreakParser = new LineBreakParser();
            BacktickParser = new BacktickParser();
            InlineCodeParser = new InlineCodeParser();
            EscapedCharParser = new EscapedCharParser();
            EntityParser = new EntityParser();
            StrWithEntitiesParser = new StrWithEntitiesParser(this);
            EscapedStringParser = new EscapedStringParser(this);
            AutolinkParser = new AutolinkParser(this);
            AutolinkEmailParser = new AutolinkEmailParser(this);
            RawHtmlParser = new RawHtmlParser();
            LinkLabelParser = new LinkLabelParser(this);
            LinkDestinationParser = new LinkDestinationParser(this);
            LinkTitleParser = new LinkTitleParser(this);
            LinkReferenceParser = new LinkReferenceParser(this);
            LinkParser = new LinkParser(this);
            ImageParser = new ImageParser(this);
            ImageReferenceParser = new ImageReferenceParser(this);
            LinkDefinitionParser = new LinkDefinitionParser(this);
            EmphasisParser = new EmphasisParser(this);
            InlineParser = new InlineParser(this);
            CommonMarkInlineParser = new CommonMarkInlineParser(this);

            // Create block parsers
            IndentedCodeParser = new IndentedCodeParser();
            LazyParagraphContinuationParser = new LazyParagraphContinuationParser();
            BlockQuoteParser = new BlockQuoteParser();
            ATXHeaderParser = new ATXHeaderParser();
            FencedCodeParser = new FencedCodeParser();
            HtmlBlockParser = new HtmlBlockParser();
            SetExtHeaderParser = new SetExtHeaderParser();
            HorizontalRuleParser = new HorizontalRuleParser();
            ListParser = new ListParser();
        }

        // Inline parsers
        public IInlineParser<LineBreak> LineBreakParser { get; set; }
        public IInlineParser<InlineString> BacktickParser { get; set; }
        public IInlineParser<InlineCode> InlineCodeParser { get; set; }
        public IInlineParser<InlineString> EscapedCharParser { get; set; }
        public IInlineParser<Entity> EntityParser { get; set; }
        public IInlineParser<Inline> EmphasisParser { get; set; }
        public IInlineParser<Link> AutolinkParser { get; set; }
        public IInlineParser<Link> AutolinkEmailParser { get; set; }
        public IInlineParser<RawHtml> RawHtmlParser { get; set; }
        public IInlineParser<Link> LinkParser { get; set; }
        public IInlineParser<LinkReference> LinkReferenceParser { get; set; }
        public IInlineParser<LinkLabel> LinkLabelParser { get; set; }
        public IInlineParser<LinkDestination> LinkDestinationParser { get; set; }
        public IInlineParser<LinkTitle> LinkTitleParser { get; set; }
        public IInlineParser<Image> ImageParser { get; set; }
        public IInlineParser<ImageReference> ImageReferenceParser { get; set; }
        public IInlineParser<Inline> StrWithEntitiesParser { get; set; }
        public IInlineParser<Inline> EscapedStringParser { get; set; }
        public IInlineParser<Link> LinkDefinitionParser { get; set; }
        public IInlineParser<Inline> InlineParser { get; set; }
        public IInlineParser<Inline> CommonMarkInlineParser { get; set; }

        // Block parsers
        public IBlockParser<IndentedCode> IndentedCodeParser { get; set; }
        public IBlockParser<Block> LazyParagraphContinuationParser { get; set; }
        public IBlockParser<BlockQuote> BlockQuoteParser { get; set; }
        public IBlockParser<ATXHeader> ATXHeaderParser { get; set; }
        public IBlockParser<FencedCode> FencedCodeParser { get; set; }
        public IBlockParser<HtmlBlock> HtmlBlockParser { get; set; }
        public IBlockParser<SetExtHeader> SetExtHeaderParser { get; set; }
        public IBlockParser<HorizontalRule> HorizontalRuleParser { get; set; }
        public IBlockParser<List> ListParser { get; set; }
    }
}
