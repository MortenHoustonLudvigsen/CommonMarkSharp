﻿using System.Collections.Generic;

namespace CommonMarkSharp.InlineParsers
{
    public static class IInlineParserExtensions
    {
        public static IEnumerable<T> ParseMany<T>(this IInlineParser<T> parser, ParserContext context, string input)
            where T : class
        {
            return parser.ParseMany(context, new Subject(input));
        }

        public static IEnumerable<T> ParseMany<T>(this IInlineParser<T> parser, ParserContext context, Subject subject)
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
