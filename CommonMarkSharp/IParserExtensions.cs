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
        public static T Parse<T>(this IParser<T> parser, Document document, string input)
            where T : class
        {
            return parser.Parse(new ParserContext(document), input);
        }

        public static T Parse<T>(this IParser<T> parser, Document document, Subject subject)
            where T : class
        {
            return parser.Parse(new ParserContext(document), subject);
        }

        public static T Parse<T>(this IParser<T> parser, ParserContext context, string input)
            where T : class
        {
            return parser.Parse(context, new Subject(input));
        }

        public static IEnumerable<T> ParseMany<T>(this IParser<T> parser, Document document, string input)
            where T : class
        {
            return parser.ParseMany(new ParserContext(document), input);
        }

        public static IEnumerable<T> ParseMany<T>(this IParser<T> parser, ParserContext context, string input)
            where T : class
        {
            return parser.ParseMany(context, new Subject(input));
        }

        public static IEnumerable<T> ParseMany<T>(this IParser<T> parser, ParserContext context, Subject subject)
            where T : class
        {
            var parts = new List<T>();
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
