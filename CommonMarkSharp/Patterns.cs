using System.Text.RegularExpressions;

namespace CommonMarkSharp
{
    public static class Patterns
    {
        public static readonly CharSet ControlChars = 
            "\x00\x01\x02\x03\x04\x05\x06\x07\x08\x09\x0A\x0B\x0C\x0D\x0E\x0F" +
            "\x10\x11\x12\x13\x14\x15\x16\x17\x18\x19\x1A\x1B\x1C\x1D\x1E\x1F\x20";

        public static readonly CharSet UpperCaseAlphas = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static readonly CharSet LowerCaseAlphas = "abcdefghijklmnopqrstuvwxyz";
        public static readonly CharSet Alphas = UpperCaseAlphas + LowerCaseAlphas;
        public static readonly CharSet Digits = "0123456789";
        public static readonly CharSet Alphanums = Alphas + Digits;
        public static readonly CharSet HexDigits = "0123456789ABCDEFabcdef";

        //// Scan an opening code fence.
        ////     [`]{3,} / [^`\n\x00]*[\n]
        ////     [~]{3,} / [^~\n\x00]*[\n]
        //public static readonly string OpenCodeFence1 = @"`{3,}/[^`\n\x00]*\n";
        //public static readonly string OpenCodeFence2 = @"~{3,}/[^~\n\x00]*\n";
        //public static readonly string OpenCodeFence = Join(OpenCodeFence1, OpenCodeFence2);
        //public static readonly Regex OpenCodeFenceRe = new Regex(Format(@"\G{0}", OpenCodeFence), ReOptions);
        public static readonly Regex OpenCodeFenceRe = RegexUtils.Create(@"\G(?:`{3,}(?!.*`)|~{3,}(?!.*~))");
        public static readonly Regex CloseCodeFenceRe = RegexUtils.Create(@"\G(?:`{3,}|~{3,})(?= *$)");
    }
}
