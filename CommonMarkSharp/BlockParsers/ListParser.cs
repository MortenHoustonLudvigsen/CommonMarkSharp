using CommonMarkSharp.Blocks;
using System.Linq;

namespace CommonMarkSharp.BlockParsers
{
    public class ListParser : IBlockParser<List>
    {
        public bool Parse(ParserContext context, Subject subject)
        {
            var saved = subject.Save();

            var indent = subject.AdvanceWhile(' ', 3);
            var ok = false;
            var spacesAfterMarker = 0;
            var data = new ListData();
            var startIndex = subject.Index;
            if (subject.Char == '*' || subject.Char == '+' || subject.Char == '-')
            {
                data.Type = "Bullet";
                data.BulletChar = subject.Char;
                subject.Advance();
                ok = true;
            }
            else if (Patterns.Digits.Contains(subject.Char))
            {
                var start = subject.TakeWhile(c => Patterns.Digits.Contains(c));
                if (subject.Char == '.' || subject.Char == ')')
                {
                    data.Type = "Ordered";
                    data.Start = int.Parse(start);
                    data.Delimiter = subject.Char;
                    subject.Advance();
                    ok = true;
                }
            }

            spacesAfterMarker = subject.AdvanceWhile(' ');

            if (!ok || spacesAfterMarker == 0 && !subject.EndOfString)
            {
                saved.Restore();
                return false;
            }

            data.Padding = subject.Index - startIndex;
            if (spacesAfterMarker >= 5 || spacesAfterMarker < 1 || subject.EndOfString)
            {
                data.Padding = data.Padding - spacesAfterMarker + 1;
            }

            saved.Restore();
            subject.AdvanceToFirstNonSpace(data.Padding);

            // list item
            data.MarkerOffset = indent;

            var list = context.Container as List;
            // add the list if needed
            if (list == null || !list.Data.Matches(data))
            {
                context.AddBlock(new List { Data = data });
            }

            // add the list item
            context.AddBlock(new ListItem { Data = data });

            return true;
        }
    }
}
