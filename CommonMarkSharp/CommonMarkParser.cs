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
            HandleInlinesVisitor = new HandleInlinesVisitor(this);
        }

        public Parsers Parsers { get; private set; }
        public HandleInlinesVisitor HandleInlinesVisitor { get; private set; }

        public Document Parse(TextReader reader)
        {
            var document = new Document { StartLine = 1 };
            string line;
            var lineNumber = 0;
            while ((line = reader.ReadLine()) != null)
            {
                lineNumber += 1;
                ProcessLine(document, line, lineNumber);
            }
            
            while (document.Tip is Block)
            {
                document.Tip.Close(this, lineNumber);
            }

            document.Accept(HandleInlinesVisitor);
            return document;
        }

        private void ProcessLine(Document document, string line, int lineNumber)
        {
            line = ExpandTabs(line);

            var groups = new string[0];
            //var subject = new LineInfo(line);
            var subject = new Subject(line);
            Block container = document;
            ListData listData;

            while (container.LastChild != null && container.LastChild.IsOpen)
            {
                container = container.LastChild;
                //subject.FindFirstNonSpace();
                if (!container.MatchNextLine(subject))
                {
                    container = container.Parent; // back up to last matching block
                    break;
                }
            }

            var lastMatchedContainer = container;

            // This function is used to finalize and close any unmatched
            // blocks.  We aren't ready to do this now, because we might
            // have a lazy paragraph continuation, in which case we don't
            // want to close unmatched blocks.  So we store this closure for
            // use later, when we have more information.
            var oldtip = document.Tip;
            Action closeUnmatchedBlocks = () =>
            {
                // finalize any blocks not matched
                while (oldtip != lastMatchedContainer)
                {
                    oldtip.Close(this, lineNumber);
                    oldtip = oldtip.Parent;
                }
                closeUnmatchedBlocks = () => { };
            };

            // Check to see if we've hit 2nd blank line; if so break out of list:
            if (subject.IsBlank && container.LastLineIsBlank)
            {
                BreakOutOfLists(document, container, lineNumber);
            }

            // Unless last matched container is a code block, try new container starts,
            // adding children to the last matched container:
            while (!(container is FencedCode || container is IndentedCode || container is HtmlBlock))
            {
                if (subject.Indent >= CODE_INDENT)
                {
                    // indented code
                    if (!(document.Tip is Paragraph) && !subject.IsBlank)
                    {
                        subject.Advance(CODE_INDENT);
                        closeUnmatchedBlocks();
                        container = document.AddBlock(this, lineNumber, new IndentedCode { StartLine = lineNumber });
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
                    container = document.AddBlock(this, lineNumber, new BlockQuote { StartLine = lineNumber });
                }
                else if (Patterns.ATXHeaderRe.IsMatch(subject.Text, subject.FirstNonSpace, out groups))
                {
                    // ATX header
                    subject.AdvanceToFirstNonSpace(groups[0].Length);
                    closeUnmatchedBlocks();

                    container = document.AddBlock(this, lineNumber, new ATXHeader(
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
                    container = document.AddBlock(this, lineNumber, new FencedCode
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
                    container = document.AddBlock(this, lineNumber, new HtmlBlock { StartLine = lineNumber });
                    // note, we don't adjust offset because the tag is part of the text
                    break;
                }
                else if (container is Paragraph && container.Strings.Count() == 1 &&
                    Patterns.SetExtHeaderRe.IsMatch(subject.Text, subject.FirstNonSpace, out groups))
                {
                    // setext header line
                    closeUnmatchedBlocks();
                    container = container.Parent.Replace(container, new SetextHeader(groups[0][0] == '=' ? 1 : 2, container.Strings.First())
                    {
                        StartLine = lineNumber
                    });
                    subject.AdvanceToEnd();
                }
                else if (Patterns.HRuleRe.IsMatch(subject.Text, subject.FirstNonSpace, out groups))
                {
                    // hrule
                    closeUnmatchedBlocks();
                    container = document.AddBlock(this, lineNumber, new HorizontalRule { StartLine = lineNumber });
                    subject.AdvanceToEnd(-1);
                    break;
                }
                else if ((listData = ParseListMarker(subject)) != null)
                {
                    // list item
                    closeUnmatchedBlocks();
                    listData.MarkerOffset = subject.Indent;
                    subject.AdvanceToFirstNonSpace(listData.Padding);

                    var list = container as List;
                    // add the list if needed
                    if (list == null || !list.Data.Matches(listData))
                    {
                        container = document.AddBlock(this, lineNumber, new List { StartLine = lineNumber, Data = listData });
                    }

                    // add the list item
                    container = document.AddBlock(this, lineNumber, new ListItem { StartLine = lineNumber, Data = listData });
                }
                else
                {
                    break;
                }

                if (container.AcceptsLines)
                {
                    // if it's a line container, it can't contain other containers
                    break;
                }
            }

            // What remains at the offset is a text line.  Add the text to the
            // appropriate container.

            // First check for a lazy paragraph continuation:
            if (document.Tip != lastMatchedContainer && !subject.IsBlank &&
                document.Tip is Paragraph && document.Tip.Strings.Any())
            {
                // lazy paragraph continuation
                //document.Tip.LastLineIsBlank = false;
                document.Tip.AddLine(subject.Rest);
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
                if (!subject.IsBlank || container is BlockQuote || container is FencedCode)
                {
                    container.LastLineIsBlank = false;
                }
                else if (container is ListItem)
                {
                    container.LastLineIsBlank = container.StartLine < lineNumber;
                }
                else
                {
                    container.LastLineIsBlank = true;
                }

                var cont = container;
                while (cont.Parent is Block)
                {
                    cont.Parent.LastLineIsBlank = false;
                    cont = cont.Parent;
                }

                if (container is IndentedCode || container is HtmlBlock)
                {
                    document.Tip.AddLine(subject.Rest);
                }
                else if (container is FencedCode)
                {
                    var fencedCode = container as FencedCode;
                    // check for closing code fence:
                    var match =
                        subject.Indent <= 3 &&
                        subject.FirstNonSpaceChar == fencedCode.Char &&
                        Patterns.CloseCodeFenceRe.IsMatch(subject.Text, subject.FirstNonSpace, out groups);

                    if (match && groups[0].Length >= fencedCode.Length)
                    {
                        // don't add closing fence to container; instead, close it:
                        container.Close(this, lineNumber);
                    }
                    else
                    {
                        document.Tip.AddLine(subject.Rest);
                    }
                }
                else if (container is ATXHeader || container is SetextHeader || container is HorizontalRule)
                {
                    // nothing to do; we already added the contents.
                }
                else
                {
                    if (container.AcceptsLines)
                    {
                        subject.AdvanceToFirstNonSpace();
                        document.Tip.AddLine(subject.Rest);
                    }
                    else if (subject.IsBlank)
                    {
                        // do nothing
                    }
                    else if (!(container is HorizontalRule || container is SetextHeader))
                    {
                        // create paragraph container for line
                        container = document.AddBlock(this, lineNumber, new Paragraph { StartLine = lineNumber });
                        subject.AdvanceToFirstNonSpace();
                        document.Tip.AddLine(subject.Rest);
                    }
                    else
                    {
                        throw new Exception(
                            "Line " + lineNumber.ToString() +
                            " with container type " + container.GetType().Name +
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
        // document to the parent of the highest list, and finalizing
        // all the lists.  (This is used to implement the "two blank lines
        // break of of all lists" feature.)
        private void BreakOutOfLists(Document document, Block block, int lineNumber)
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
                    block.Close(this, lineNumber);
                    block = block.Parent;
                }
                lastList.Close(this, lineNumber);
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
