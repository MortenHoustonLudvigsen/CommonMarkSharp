using CommonMarkSharp;
using CommonMarkSharp.Blocks;
using System;
using System.Linq;

namespace Examples
{
    public class HtmlPageRenderer : HtmlRenderer
    {
        public override void Visit(Document document)
        {
            WriteLine("<html>");
            WriteLine("<head>");
            WriteLine("<title>A title</title>");
            WriteLine("</head>");
            WriteLine("<body>");
            base.Visit(document);
            WriteLine("</body>");
            WriteLine("</html>");
        }
    }
}