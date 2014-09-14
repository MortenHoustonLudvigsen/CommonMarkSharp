using CommonMarkSharp.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.BlockParsers
{
    public static class IBlockParserExtensions
    {
        public static IEnumerable<T> ParseMany<T>(this IBlockParser<T> parser, ParserContext context, string input)
            where T : class
        {
            return parser.ParseMany(context, new Subject(input));
        }

        public static IEnumerable<T> ParseMany<T>(this IBlockParser<T> parser, ParserContext context, Subject subject)
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
