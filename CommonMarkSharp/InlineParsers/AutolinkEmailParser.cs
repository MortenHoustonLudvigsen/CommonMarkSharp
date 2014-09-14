using CommonMarkSharp.Inlines;
using System;
using System.Collections.Generic;

namespace CommonMarkSharp.InlineParsers
{
    public class AutolinkEmailParser : IInlineParser<Link>
    {
        private const string _alphas = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private const string _digits = "0123456789";
        private const string _alphanums = _alphas + _digits;
        private static readonly HashSet<char> _emailNameChars = new HashSet<char>(_alphanums + ".!#$%&'*+/=?^_`{|}~-");
        private static readonly HashSet<char> _domainNameStartChars = new HashSet<char>(_alphanums);
        private static readonly HashSet<char> _domainNameChars = new HashSet<char>(_alphanums + "-");

        public AutolinkEmailParser(Parsers parsers)
        {
            Parsers = parsers;
        }

        public Parsers Parsers { get; private set; }
        public Lazy<IInlineParser<InlineString>> _emailNameParser { get; private set; }
        public Lazy<IInlineParser<InlineString>> _domainNameParser { get; private set; }

        public string StartsWithChars
        {
            get { return "<"; }
        }

        public bool CanParse(Subject subject)
        {
            return subject.Char == '<';
        }

        public Link Parse(ParserContext context, Subject subject)
        {
            if (!CanParse(subject)) return null;

            var saved = subject.Save();
            subject.Advance();

            var emailStart = subject.Save();

            if (MatchEmailName(subject) && subject.Char == '@')
            {
                subject.Advance();

                if (MatchDomainName(subject) && subject.Char == '>')
                {
                    var email = emailStart.GetLiteral();
                    subject.Advance();
                    return new Link(
                        new LinkLabel(email, new[] { new InlineString(email) }),
                        new LinkDestination("mailto:" + email),
                        new LinkTitle()
                    );
                }
            }

            saved.Restore();
            return null;
        }

        private static bool MatchEmailName(Subject subject)
        {
            return subject.AdvanceWhile(c => _emailNameChars.Contains(c)) > 0;
        }

        private bool MatchDomainName(Subject subject)
        {
            if (!MatchDomainNamePart(subject))
            {
                return false;
            }
            while (subject.Char == '.')
            {
                subject.Advance();
                if (!MatchDomainNamePart(subject))
                {
                    return false;
                }
            }
            return true;
        }

        private bool MatchDomainNamePart(Subject subject)
        {
            if (!_domainNameStartChars.Contains(subject.Char))
            {
                return false;
            }
            subject.AdvanceWhile(c => _domainNameChars.Contains(c), 62);
            return _domainNameStartChars.Contains(subject[-1]);
        }
    }
}
