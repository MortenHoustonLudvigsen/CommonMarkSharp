using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Inlines
{
    public class Entity : InlineString
    {
        public Entity(string entity)
            : base(entity)
        {
        }
    }
}
