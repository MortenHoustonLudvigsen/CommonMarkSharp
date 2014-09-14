using CommonMarkSharp.Blocks;
using System.IO;

namespace CommonMarkSharp
{
    public class CommonMark
    {
        public CommonMark()
        {
            Init();
        }

        public Parsers Parsers { get; protected set; }
        public CommonMarkParser CommonMarkParser { get; protected set; }
        public TextRenderer Renderer { get; set; }

        public virtual void Init()
        {
            Parsers = new Parsers();
            CommonMarkParser = new CommonMarkParser(Parsers);
            Renderer = new HtmlRenderer();
        }

        public Document Parse(string commonMark)
        {
            using (var reader = new StringReader(commonMark))
            {
                return Parse(reader);
            }
        }

        public Document Parse(TextReader reader)
        {
            return CommonMarkParser.Parse(reader);
        }

        public string RenderAsHtml(Document document)
        {
            return Renderer.Render(document);
        }

        public string RenderAsHtml(string commonMark)
        {
            return Renderer.Render(Parse(commonMark));
        }

        public string RenderAsHtml(TextReader reader)
        {
            return Renderer.Render(Parse(reader));
        }

        public void RenderAsHtml(Document document, TextWriter writer)
        {
            Renderer.Render(document, writer);
        }

        public void RenderAsHtml(string commonMark, TextWriter writer)
        {
            Renderer.Render(Parse(commonMark), writer);
        }

        public void RenderAsHtml(TextReader reader, TextWriter writer)
        {
            Renderer.Render(Parse(reader), writer);
        }
    }
}
