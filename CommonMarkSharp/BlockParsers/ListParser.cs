using CommonMarkSharp.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.BlockParsers
{
    //else if ((data = ParseListMarker(subject)) != null)
    //{
    //    // list item
    //    data.MarkerOffset = subject.Indent;
    //    subject.AdvanceToFirstNonSpace(data.Padding);

    //    var list = context.Container as List;
    //    // add the list if needed
    //    if (list == null || !list.Data.Matches(data))
    //    {
    //        context.AddBlock(new List { Data = data });
    //    }

    //    // add the list item
    //    context.AddBlock(new ListItem { Data = data });
    //}

    //private static Regex _bulletListMarkerRe = RegexUtils.Create(@"^[*+-]( +|$)");
    //private static Regex _orderedListMarkerRe = RegexUtils.Create(@"^(\d+)([.)])( +|$)");
    //private ListData ParseListMarker(Subject subject)
    //{
    //    var saved = subject.Save();
    //    subject.AdvanceToFirstNonSpace();
    //    var rest = subject.Rest;
    //    var groups = new string[0];
    //    var spacesAfterMarker = 0;
    //    var data = new ListData();
    //    if (rest == "" || !subject.EndOfString && Patterns.HRuleRe.IsMatch(subject.Text, subject.FirstNonSpace))
    //    {
    //        saved.Restore();
    //        return null;
    //    }
    //    if (_bulletListMarkerRe.IsMatch(rest, out groups))
    //    {
    //        spacesAfterMarker = groups[1].Length;
    //        data.Type = "Bullet";
    //        data.BulletChar = groups[0][0];
    //    }
    //    else if (_orderedListMarkerRe.IsMatch(rest, out groups))
    //    {
    //        spacesAfterMarker = groups[3].Length;
    //        data.Type = "Ordered";
    //        data.Start = int.Parse(groups[1]);
    //        data.Delimiter = groups[2];
    //    }
    //    else
    //    {
    //        saved.Restore();
    //        return null;
    //    }
    //    var itemIsBlank = groups[0].Length == rest.Length;
    //    if (spacesAfterMarker >= 5 || spacesAfterMarker < 1 || itemIsBlank)
    //    {
    //        data.Padding = groups[0].Length - spacesAfterMarker + 1;
    //    }
    //    else
    //    {
    //        data.Padding = groups[0].Length;
    //    }
    //    saved.Restore();
    //    return data;
    //}
    public class ListParser : IBlockParser<List>
    {
        public bool Parse(ParserContext context, Subject subject)
        {
            var saved = subject.Save();

            var indent = subject.AdvanceWhile(c => c == ' ', 3);
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

            spacesAfterMarker = subject.AdvanceWhile(c => c == ' ');

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
