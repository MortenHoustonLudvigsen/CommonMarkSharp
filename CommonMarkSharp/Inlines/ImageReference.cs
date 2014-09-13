using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Inlines
{
    public class ImageReference : Inline
    {
        public ImageReference(LinkReference linkReference)
        {
            LinkReference = linkReference;
        }

        public LinkReference LinkReference { get; private set; }
    }
}
