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
    public class AutolinkEmailParser : RegexParser<Link>
    {
        // Try to match email autolink after first <.
        //     [a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+
        //       [@]
        //           [a-zA-Z0-9]([a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?
        //       ([.][a-zA-Z0-9]([a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*
        //       [>]
        public static readonly string AutolinkEmailName = @"[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+";
        public static readonly string AutolinkEmailDomainNamePart = @"[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?";
        public static readonly string AutolinkEmail = string.Format(@"{0}@{1}(?:\.{1})*", AutolinkEmailName, AutolinkEmailDomainNamePart);
        public static readonly Regex AutolinkEmailRe = new Regex(string.Format(@"\G<({0})>", AutolinkEmail), RegexUtils.Options);

        public AutolinkEmailParser(Parsers parsers)
            : base(AutolinkEmailRe)
        {
            Parsers = parsers;
            StartsWithChars = "<";
        }

        public Parsers Parsers { get; private set; }

        protected override Link HandleMatch(ParserContext context, string[] groups)
        {
            return new Link(
                new LinkLabel(groups[1], Parsers.StrWithEntitiesParser.ParseMany(context, new Subject(groups[1]))),
                new LinkDestination(string.Format("mailto:{0}", groups[1])),
                new LinkTitle()
            );
        }
    }
}
