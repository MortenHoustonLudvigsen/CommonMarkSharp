using CommonMarkSharp.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.BlockParsers
{
    public class IndentedCodeParser : IBlockParser<IndentedCode>
    {
        public string StartsWithChars
        {
            get { return " "; }
        }

        public bool CanParse(Subject subject)
        {
            return subject.Char == ' ';
        }

        public IndentedCode Parse(ParserContext context, Subject subject)
        {
            if (!CanParse(subject)) return null;

            // A lazy paragraph continuation
            if (context.Tip is Paragraph) return null;

            // Do not match blank line
            if (subject.IsBlank) return null;

            if (subject.Indent >= CommonMarkParser.CODE_INDENT)
            {
                subject.Advance(CommonMarkParser.CODE_INDENT);
                return new IndentedCode();
            }

            return null;
        }
    }
}
