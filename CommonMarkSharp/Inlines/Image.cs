using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Inlines
{
    public class Image : Inline
    {
        public Image(Link link)
        {
            Link = link;
        }

        public Link Link { get; private set; }
    }
}
