using CommonMarkSharp.Blocks;
using CommonMarkSharp.Inlines;
using CommonMarkSharp.InlineParsers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonMarkSharp
{
    public class CommonMarkParser
    {
        public const int CODE_INDENT = 4;

        public CommonMarkParser(Parsers parsers)
        {
            Parsers = parsers;
        }

        public Parsers Parsers { get; private set; }

        public Document Parse(TextReader reader)
        {
            var context = new ParserContext(Parsers, new Document { StartLine = 1 });
            string line;
            var lineNumber = 0;
            while ((line = reader.ReadLine()) != null)
            {
                lineNumber += 1;
                ProcessLine(context, line, lineNumber);
            }

            while (context.Tip is Block)
            {
                context.Tip.Close(context, lineNumber);
            }

            context.Document.Accept(new HandleInlinesVisitor(context));
            return context.Document;
        }

        private void ProcessLine(ParserContext context, string line, int lineNumber)
        {
            line = ExpandTabs(line);

            var groups = new string[0];
            //var subject = new LineInfo(line);
            var subject = new Subject(line);
            ListData listData;

            context.Container = context.Document;
            while (context.Container.LastChild != null && context.Container.LastChild.IsOpen)
            {
                context.Container = context.Container.LastChild;
                //subject.FindFirstNonSpace();
                if (!context.Container.MatchNextLine(subject))
                {
                    context.Container = context.Container.Parent; // back up to last matching block
                    break;
                }
            }

            var lastMatchedContainer = context.Container;

            // This function is used to finalize and close any unmatched
            // blocks.  We aren't ready to do this now, because we might
            // have a lazy paragraph continuation, in which case we don't
            // want to close unmatched blocks.  So we store this closure for
            // use later, when we have more information.
            var oldtip = context.Tip;
            Action closeUnmatchedBlocks = () =>
            {
                // finalize any blocks not matched
                while (oldtip != lastMatchedContainer)
                {
                    oldtip.Close(context, lineNumber);
                    oldtip = oldtip.Parent;
                }
                closeUnmatchedBlocks = () => { };
            };

            // Check to see if we've hit 2nd blank line; if so break out of list:
            if (subject.IsBlank && context.Container.LastLineIsBlank)
            {
                BreakOutOfLists(context, context.Container, lineNumber);
            }

            // Unless last matched context.Container is a code block, try new context.Container starts,
            // adding children to the last matched context.Container:
            while (!(context.Container is FencedCode || context.Container is IndentedCode || context.Container is HtmlBlock))
            {
                if (subject.Indent >= CODE_INDENT)
                {
                    // indented code
                    if (!(context.Tip is Paragraph) && !subject.IsBlank)
                    {
                        subject.Advance(CODE_INDENT);
                        closeUnmatchedBlocks();
                        context.Container = context.AddBlock(lineNumber, new IndentedCode { StartLine = lineNumber });
                    }
                    else
                    { // indent > 4 in a lazy paragraph continuation
                        break;
                    }
                }
                else if (subject.FirstNonSpaceChar == '>')
                {
                    // blockquote
                    subject.AdvanceToFirstNonSpace(1);
                    // optional following space
                    if (subject.Char == ' ')
                    {
                        subject.Advance();
                    }
                    closeUnmatchedBlocks();
                    context.Container = context.AddBlock(lineNumber, new BlockQuote { StartLine = lineNumber });
                }
                else if (Patterns.ATXHeaderRe.IsMatch(subject.Text, subject.FirstNonSpace, out groups))
                {
                    // ATX header
                    subject.AdvanceToFirstNonSpace(groups[0].Length);
                    closeUnmatchedBlocks();

                    context.Container = context.AddBlock(lineNumber, new ATXHeader(
                        groups[1].Length,
                        // remove trailing ###s:
                        Patterns.ATXHeaderRemoveTrailingHashRe.Replace(subject.Rest, "$1")
                    ) { StartLine = lineNumber });
                    break;
                }
                else if (Patterns.OpenCodeFenceRe.IsMatch(subject.Text, subject.FirstNonSpace, out groups))
                {
                    // fenced code block
                    var fenceLength = groups[0].Length;
                    closeUnmatchedBlocks();
                    context.Container = context.AddBlock(lineNumber, new FencedCode
                    {
                        StartLine = lineNumber,
                        Length = fenceLength,
                        Char = groups[0][0],
                        Offset = subject.FirstNonSpace - subject.Index
                    });
                    subject.AdvanceToFirstNonSpace(fenceLength);
                    break;
                }
                else if (Patterns.HtmlBlockTagRe.IsMatch(subject.Text, subject.FirstNonSpace, out groups))
                {
                    // html block
                    closeUnmatchedBlocks();
                    context.Container = context.AddBlock(lineNumber, new HtmlBlock { StartLine = lineNumber });
                    // note, we don't adjust offset because the tag is part of the text
                    break;
                }
                else if (context.Container is Paragraph && context.Container.Strings.Count() == 1 &&
                    Patterns.SetExtHeaderRe.IsMatch(subject.Text, subject.FirstNonSpace, out groups))
                {
                    // setext header line
                    closeUnmatchedBlocks();
                    context.Container = context.Container.Parent.Replace(context, context.Container, new SetextHeader(groups[0][0] == '=' ? 1 : 2, context.Container.Strings.First())
                    {
                        StartLine = lineNumber
                    });
                    subject.AdvanceToEnd();
                }
                else if (Patterns.HRuleRe.IsMatch(subject.Text, subject.FirstNonSpace, out groups))
                {
                    // hrule
                    closeUnmatchedBlocks();
                    context.Container = context.AddBlock(lineNumber, new HorizontalRule { StartLine = lineNumber });
                    subject.AdvanceToEnd(-1);
                    break;
                }
                else if ((listData = ParseListMarker(subject)) != null)
                {
                    // list item
                    closeUnmatchedBlocks();
                    listData.MarkerOffset = subject.Indent;
                    subject.AdvanceToFirstNonSpace(listData.Padding);

                    var list = context.Container as List;
                    // add the list if needed
                    if (list == null || !list.Data.Matches(listData))
                    {
                        context.Container = context.AddBlock(lineNumber, new List { StartLine = lineNumber, Data = listData });
                    }

                    // add the list item
                    context.Container = context.AddBlock(lineNumber, new ListItem { StartLine = lineNumber, Data = listData });
                }
                else
                {
                    break;
                }

                if (context.Container.AcceptsLines)
                {
                    // if it's a line context.Container, it can't contain other containers
                    break;
                }
            }

            // What remains at the offset is a text line.  Add the text to the
            // appropriate context.Container.

            // First check for a lazy paragraph continuation:
            if (context.Tip != lastMatchedContainer && !subject.IsBlank &&
                context.Tip is Paragraph && context.Tip.Strings.Any())
            {
                // lazy paragraph continuation
                //context.Tip.LastLineIsBlank = false;
                context.Tip.AddLine(subject.Rest);
            }
            else
            {
                // not a lazy continuation

                // finalize any blocks not matched
                closeUnmatchedBlocks();

                // Block quote lines are never blank as they start with >
                // and we don't count blanks in fenced code for purposes of tight/loose
                // lists or breaking out of lists.  We also don't set last_line_blank
                // on an empty list item.
                if (!subject.IsBlank || context.Container is BlockQuote || context.Container is FencedCode)
                {
                    context.Container.LastLineIsBlank = false;
                }
                else if (context.Container is ListItem)
                {
                    context.Container.LastLineIsBlank = context.Container.StartLine < lineNumber;
                }
                else
                {
                    context.Container.LastLineIsBlank = true;
                }

                var cont = context.Container;
                while (cont.Parent is Block)
                {
                    cont.Parent.LastLineIsBlank = false;
                    cont = cont.Parent;
                }

                if (context.Container is IndentedCode || context.Container is HtmlBlock)
                {
                    context.Tip.AddLine(subject.Rest);
                }
                else if (context.Container is FencedCode)
                {
                    var fencedCode = context.Container as FencedCode;
                    // check for closing code fence:
                    var match =
                        subject.Indent <= 3 &&
                        subject.FirstNonSpaceChar == fencedCode.Char &&
                        Patterns.CloseCodeFenceRe.IsMatch(subject.Text, subject.FirstNonSpace, out groups);

                    if (match && groups[0].Length >= fencedCode.Length)
                    {
                        // don't add closing fence to context.Container; instead, close it:
                        context.Container.Close(context, lineNumber);
                    }
                    else
                    {
                        context.Tip.AddLine(subject.Rest);
                    }
                }
                else if (context.Container is ATXHeader || context.Container is SetextHeader || context.Container is HorizontalRule)
                {
                    // nothing to do; we already added the contents.
                }
                else
                {
                    if (context.Container.AcceptsLines)
                    {
                        subject.AdvanceToFirstNonSpace();
                        context.Tip.AddLine(subject.Rest);
                    }
                    else if (subject.IsBlank)
                    {
                        // do nothing
                    }
                    else if (!(context.Container is HorizontalRule || context.Container is SetextHeader))
                    {
                        // create paragraph context.Container for line
                        context.Container = context.AddBlock(lineNumber, new Paragraph { StartLine = lineNumber });
                        subject.AdvanceToFirstNonSpace();
                        context.Tip.AddLine(subject.Rest);
                    }
                    else
                    {
                        throw new Exception(
                            "Line " + lineNumber.ToString() +
                            " with context.Container type " + context.Container.GetType().Name +
                            " did not match any condition."
                        );
                    }
                }
            }
        }

        private static Regex _bulletListMarkerRe = RegexUtils.Create(@"^[*+-]( +|$)");
        private static Regex _orderedListMarkerRe = RegexUtils.Create(@"^(\d+)([.)])( +|$)");
        private ListData ParseListMarker(Subject subject)
        {
            var savedSubject = subject.Save();
            subject.AdvanceToFirstNonSpace();
            var rest = subject.Rest;
            var groups = new string[0];
            var spacesAfterMarker = 0;
            var data = new ListData();
            if (rest == "" || !subject.EndOfString && Patterns.HRuleRe.IsMatch(subject.Text, subject.FirstNonSpace))
            {
                savedSubject.Restore();
                return null;
            }
            if (_bulletListMarkerRe.IsMatch(rest, out groups))
            {
                spacesAfterMarker = groups[1].Length;
                data.Type = "Bullet";
                data.BulletChar = groups[0][0];
            }
            else if (_orderedListMarkerRe.IsMatch(rest, out groups))
            {
                spacesAfterMarker = groups[3].Length;
                data.Type = "Ordered";
                data.Start = int.Parse(groups[1]);
                data.Delimiter = groups[2];
            }
            else
            {
                savedSubject.Restore();
                return null;
            }
            var itemIsBlank = groups[0].Length == rest.Length;
            if (spacesAfterMarker >= 5 || spacesAfterMarker < 1 || itemIsBlank)
            {
                data.Padding = groups[0].Length - spacesAfterMarker + 1;
            }
            else
            {
                data.Padding = groups[0].Length;
            }
            savedSubject.Restore();
            return data;
        }

        // Break out of all containing lists, resetting the tip of the
        // context.Document to the parent of the highest list, and finalizing
        // all the lists.  (This is used to implement the "two blank lines
        // break of of all lists" feature.)
        private void BreakOutOfLists(ParserContext context, Block block, int lineNumber)
        {
            var b = block;
            Block lastList = null;
            do
            {
                if (b is List)
                {
                    lastList = b;
                }
                b = b.Parent;
            } while (b != null);

            if (lastList != null)
            {
                while (block != lastList)
                {
                    block.Close(context, lineNumber);
                    block = block.Parent;
                }
                lastList.Close(context, lineNumber);
            }
        }

        private string ExpandTabs(string line, int tabWidth = 4)
        {
            var result = (StringBuilder)null;
            var pos = 0;
            var start = 0;
            for (var i = 0; i < line.Length; i++)
            {
                var c = line[i];
                if (c == '\t')
                {
                    if (result == null)
                    {
                        result = new StringBuilder(line.Length + 30);
                    }
                    result.Append(line, start, i - start);
                    var count = tabWidth - pos % tabWidth;
                    pos += count;
                    start = i + 1;
                    result.Append(new string(' ', count));
                }
                else
                {
                    pos += 1;
                }
            }
            if (result != null)
            {
                result.Append(line, start, line.Length - start);
                return result.ToString();
            }
            return line;
        }
    }
}
