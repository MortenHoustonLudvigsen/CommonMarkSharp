using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Parsing.Blocks
{
    public class SetextHeader : Header
    {
        public SetextHeader(int level, string contents)
            : base(level, contents)
        {
        }

        public override void Close(CommonMarkParser parser, int lineNumber)
        {
            base.Close(parser, lineNumber);
            Contents = string.Join("\n", Strings);
        }
    }
}
