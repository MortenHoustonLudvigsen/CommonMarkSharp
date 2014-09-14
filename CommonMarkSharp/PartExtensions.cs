using System.Collections.Generic;

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
