namespace CommonMarkSharp.InlineParsers
{
    public interface IInlineParser<out T>
        where T: class
    {
        string StartsWithChars { get; }
        bool CanParse(Subject subject);
        T Parse(ParserContext context, Subject subject);
    }
}
