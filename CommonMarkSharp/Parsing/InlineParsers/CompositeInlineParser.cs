using CommonMarkSharp.Parsing.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Parsing.InlineParsers
{
    public class CompositeInlineParser : CompositeParser<Inline>
    {
        public CompositeInlineParser(bool parseOthersAsStrings, params IParser<Inline>[] parsers)
            : base(parsers)
        {
            ParseOthersAsStrings = parseOthersAsStrings;
            _othersParser = new Lazy<IParser<InlineString>>(() =>
            {
                return new AnyParser(base.StartsWithChars);
            });
            _startsWithChars = new Lazy<string>(() => ParseOthersAsStrings ? null : base.StartsWithChars);
        }

        public CompositeInlineParser(params IParser<Inline>[] parsers)
            : this(false, parsers)
        {
        }

        private Lazy<IParser<InlineString>> _othersParser;
        public bool ParseOthersAsStrings { get; private set; }

        private readonly Lazy<string> _startsWithChars;
        public override string StartsWithChars { get { return _startsWithChars.Value; } }

        public override Inline Parse(ParserContext context, Subject subject)
        {
            if (!this.CanParse(subject)) return null;
            var savedSubject = subject.Save();
            var inline = base.Parse(context, subject);
            if (inline == null && ParseOthersAsStrings)
            {
                inline = _othersParser.Value.Parse(context, subject);
            }
            if (inline != null)
            {
                return inline;
            }
            savedSubject.Restore();
            return null;
        }
    }
}
