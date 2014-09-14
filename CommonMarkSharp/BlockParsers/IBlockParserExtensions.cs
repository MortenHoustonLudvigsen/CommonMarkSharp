namespace CommonMarkSharp.BlockParsers
{
    public static class IBlockParserExtensions
    {
        public static int ParseMany<T>(this IBlockParser<T> parser, ParserContext context, string input)
            where T : class
        {
            return parser.ParseMany(context, new Subject(input));
        }

        public static int ParseMany<T>(this IBlockParser<T> parser, ParserContext context, Subject subject)
            where T : class
        {
            var count = 0;
            while (!subject.EndOfString)
            {
                if (!parser.Parse(context, subject))
                {
                    return count;
                }
                count += 1;
            }
            return count;
        }
    }
}
