﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Inlines
{
    public class RawHtml : InlineString
    {
        public RawHtml(string html)
            : base(html)
        {
        }
    }
}
