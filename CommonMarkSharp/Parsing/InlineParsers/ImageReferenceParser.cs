using CommonMarkSharp.Parsing.Blocks;
using CommonMarkSharp.Parsing.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Parsing.InlineParsers
{
    public class ImageReferenceParser : IParser<ImageReference>
    {
        public ImageReferenceParser(Parsers parsers)
        {
            Parsers = parsers;
        }

        public Parsers Parsers { get; private set; }

        public string StartsWithChars { get { return "!"; } }

        public ImageReference Parse(ParserContext context, Subject subject)
        {
            if (!this.CanParse(subject)) return null;

            var savedSubject = subject.Save();
            subject.Advance();

            var reference = Parsers.LinkReferenceParser.Parse(context, subject);
            if (reference != null)
            {
                return new ImageReference(reference);
            }

            savedSubject.Restore();
            return null;
        }
    }
}
