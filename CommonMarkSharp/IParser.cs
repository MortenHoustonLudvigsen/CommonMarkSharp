using CommonMarkSharp.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp
{
    public interface IParser<out T>
    {
        string StartsWithChars { get; }
        T Parse(ParserContext context, Subject subject);
    }
}
