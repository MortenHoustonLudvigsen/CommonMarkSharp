using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Blocks
{
    public class ListData
    {
        public string Type { get; set; }
        public char BulletChar { get; set; }
        public int? Start { get; set; }
        public string Delimiter { get; set; }
        public int Padding { get; set; }
        public int MarkerOffset { get; set; }

        public bool Matches(ListData other)
        {
            return other.Type == Type &&
                   other.Delimiter == Delimiter &&
                   other.BulletChar == BulletChar;
        }
    }
}
