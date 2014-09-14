namespace CommonMarkSharp.Inlines
{
    public class InlineCode : Inline
    {
        public InlineCode(string code)
        {
            Code = code.Trim();
        }

        public string Code { get; private set; }
    }
}
