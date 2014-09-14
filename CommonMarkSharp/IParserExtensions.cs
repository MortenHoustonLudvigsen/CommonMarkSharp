using CommonMarkSharp.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp
{
    public static class IParserExtensions
    {
        public static TPart Parse<TPart>(this IParser<TPart> parser, Document document, string input)
            where TPart : Part
        {
            return parser.Parse(new ParserContext(document), input);
        }

        public static TPart Parse<TPart>(this IParser<TPart> parser, Document document, Subject subject)
            where TPart : Part
        {
            return parser.Parse(new ParserContext(document), subject);
        }

        public static TPart Parse<TPart>(this IParser<TPart> parser, ParserContext context, string input)
            where TPart : Part
        {
            return parser.Parse(context, new Subject(input));
        }

        public static IEnumerable<TPart> ParseMany<TPart>(this IParser<TPart> parser, Document document, string input)
            where TPart : Part
        {
            return parser.ParseMany(new ParserContext(document), input);
        }

        public static IEnumerable<TPart> ParseMany<TPart>(this IParser<TPart> parser, ParserContext context, string input)
            where TPart : Part
        {
            return parser.ParseMany(context, new Subject(input));
        }

        public static IEnumerable<TPart> ParseMany<TPart>(this IParser<TPart> parser, ParserContext context, Subject subject)
            where TPart : Part
        {
            var parts = new List<TPart>();
            while (!subject.EndOfString)
            {
                var part = parser.Parse(context, subject);
                if (part == null)
                {
                    break;
                }
                parts.Add(part);
            }
            return parts;
        }
    }
}
