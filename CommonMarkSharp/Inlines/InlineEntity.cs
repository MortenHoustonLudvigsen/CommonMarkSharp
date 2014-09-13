using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Inlines
{
    public class InlineEntity : InlineString
    {
        public InlineEntity(string entity)
            : base(entity)
        {
        }

        public string Entity { get { return Value; } }
    }
}
