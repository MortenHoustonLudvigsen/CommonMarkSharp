namespace CommonMarkSharp.BlockParsers
{
    public interface IBlockParser<out T>
        where T: class
    {
        bool Parse(ParserContext context, Subject subject);
    }
}
