using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonMarkSharp
{
    public class LineInfo
    {
        private static Regex _leadingSpaceRe = new Regex(@"[^ ]");

        public LineInfo(string line)
        {
            Line = line;
            Offset = 0;
            FindFirstNonSpace();
        }

        public string Line { get; set; }
        public int FirstNonSpace { get; set; }
        public int Offset { get; set; }
        public bool Blank { get; set; }
        public int Indent { get; set; }
        public char this[int index]
        {
            get
            {
                if (index >= 0 && index < Line.Length)
                {
                    return Line[index];
                }
                return char.MinValue;
            }
        }

        public string FromIndex(int index)
        {
            return index >= 0 && index < Line.Length ? Line.Substring(index) : "";
        }

        public string FromNonSpace
        {
            get { return FromIndex(FirstNonSpace); }
        }

        public string FromOffset
        {
            get { return FromIndex(Offset); }
        }

        public void FindFirstNonSpace()
        {
            FirstNonSpace = Offset;
            while (FirstNonSpace < Line.Length && Line[FirstNonSpace] == ' ')
            {
                FirstNonSpace += 1;
            }
            Blank = FirstNonSpace == Line.Length;
            Indent = FirstNonSpace - Offset;
        }
    }
}
