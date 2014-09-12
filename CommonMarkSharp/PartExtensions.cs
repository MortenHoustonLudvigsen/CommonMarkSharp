using CommonMarkSharp.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp
{
    public static class PartExtensions
    {
        public static void Accept(this IEnumerable<Part> parts, CommonMarkVisitor visitor)
        {
            if (parts != null)
            {
                foreach (var part in parts)
                {
                    part.Accept(visitor);
                }
            }
        }
    }
}
