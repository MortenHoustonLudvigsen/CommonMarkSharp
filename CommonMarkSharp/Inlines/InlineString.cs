namespace CommonMarkSharp.Inlines
{
    public class InlineString : Inline
    {
        public InlineString(params char[] chars)
            : this(new string(chars))
        {
        }

        public InlineString(string str)
        {
            Value = str;
        }

        public string Value { get; private set; }
    }
}
