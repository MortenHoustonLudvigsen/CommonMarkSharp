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
    public class ImageParser : IParser<Image>
    {
        public ImageParser(Parsers parsers)
        {
            Parsers = parsers;
        }

        public Parsers Parsers { get; private set; }

        public string StartsWithChars { get { return "!"; } }

        public Image Parse(ParserContext context, Subject subject)
        {
            if (!this.CanParse(subject)) return null;

            var savedSubject = subject.Save();
            subject.Advance();

            var link = Parsers.LinkParser.Parse(context, subject);
            if (link != null)
            {
                return new Image(link);
            }

            savedSubject.Restore();
            return null;
        }
    }
}
