namespace CommonMarkSharp.InlineParsers
{
    public interface IInlineParser<out T>
        where T: class
    {
        string StartsWithChars { get; }
        T Parse(ParserContext context, Subject subject);
    }
}
