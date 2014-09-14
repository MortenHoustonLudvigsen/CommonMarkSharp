using CommonMarkSharp.Inlines;
using System.Collections.Generic;

namespace CommonMarkSharp.Blocks
{
    public abstract class LeafBlockWithInlines : LeafBlock
    {
        public IEnumerable<Inline> Inlines { get; set; }
    }
}
