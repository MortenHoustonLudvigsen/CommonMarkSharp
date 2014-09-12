﻿using CommonMarkSharp.Parsing.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Parsing.Blocks
{
    public class Header : LeafBlock
    {
        public Header(int level, string contents)
        {
            Level = level;
            AddLine(contents);
        }

        public int Level { get; set; }
        public IEnumerable<Inline> Inlines { get; set; }
        public override bool AcceptsLines { get { return true; } }

        public override bool MatchNextLine(LineInfo lineInfo)
        {
            return false;
        }
    }
}