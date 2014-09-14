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
    public class ImageParser : IInlineParser<Image>
    {
        public ImageParser(Parsers parsers)
        {
            Parsers = parsers;
        }

        public Parsers Parsers { get; private set; }

        public string StartsWithChars { get { return "!"; } }

        public bool CanParse(Subject subject)
        {
            return subject.Char == '!';
        }

        public Image Parse(ParserContext context, Subject subject)
        {
            if (!CanParse(subject)) return null;

            var saved = subject.Save();
            subject.Advance();

            var link = Parsers.LinkParser.Parse(context, subject);
            if (link != null)
            {
                return new Image(link);
            }

            saved.Restore();
            return null;
        }
    }
}
