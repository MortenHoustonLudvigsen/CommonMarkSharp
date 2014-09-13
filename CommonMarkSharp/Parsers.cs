using CommonMarkSharp.Inlines;
using CommonMarkSharp.InlineParsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonMarkSharp.Blocks;

namespace CommonMarkSharp
{
    public class Parsers
    {
        public Parsers()
        {
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
        }

        public IParser<LineBreak> LineBreakParser { get; set; }
        public IParser<InlineString> BacktickParser { get; set; }
        public IParser<InlineCode> InlineCodeParser { get; set; }
        public IParser<InlineString> EscapedCharParser { get; set; }
        public IParser<InlineEntity> EntityParser { get; set; }
        public IParser<Inline> EmphasisParser { get; set; }
        public IParser<Link> AutolinkParser { get; set; }
        public IParser<Link> AutolinkEmailParser { get; set; }
        public IParser<InlineRawHtml> RawHtmlParser { get; set; }
        public IParser<Link> LinkParser { get; set; }
        public IParser<LinkReference> LinkReferenceParser { get; set; }
        public IParser<LinkLabel> LinkLabelParser { get; set; }
        public IParser<LinkDestination> LinkDestinationParser { get; set; }
        public IParser<LinkTitle> LinkTitleParser { get; set; }
        public IParser<Image> ImageParser { get; set; }
        public IParser<ImageReference> ImageReferenceParser { get; set; }
        public IParser<Inline> StrWithEntitiesParser { get; set; }
        public IParser<Inline> EscapedStringParser { get; set; }
        public IParser<Link> LinkDefinitionParser { get; set; }
        public IParser<Inline> InlineParser { get; set; }
        public IParser<Inline> CommonMarkInlineParser { get; set; }
    }
}
