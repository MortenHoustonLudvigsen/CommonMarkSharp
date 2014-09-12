using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Parsing.Inlines
{
    public class SoftBreak : LineBreak
    {
        public override string ToString()
        {
            return "\n";
        }
    }
}
