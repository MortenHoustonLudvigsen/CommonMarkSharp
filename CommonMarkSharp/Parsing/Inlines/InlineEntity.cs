using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Parsing.Inlines
{
    public class InlineEntity : Inline
    {
        public InlineEntity(string entity)
        {
            Entity = entity;
        }

        public string Entity { get; set; }

        public override string ToString()
        {
            return Entity;
        }
    }
}
