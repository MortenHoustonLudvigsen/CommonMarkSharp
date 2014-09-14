using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Blocks
{
    public class SetextHeader : Header
    {
        public SetextHeader(int level, string contents)
            : base(level, contents)
        {
        }

        public override void Close(ParserContext context, int lineNumber)
        {
            base.Close(context, lineNumber);
            Contents = string.Join("\n", Strings);
        }
    }
}
