using CommonMarkSharp.Blocks;
using CommonMarkSharp.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp
{
    public abstract class Part
    {
        public virtual void Accept(CommonMarkVisitor visitor)
        {
            visitor.VisitPart(this);
        }
    }
}
