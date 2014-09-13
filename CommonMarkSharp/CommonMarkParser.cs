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

        private Regex _leadingSpaceRe = new Regex(@"\G +");
        private void ProcessLine(Document document, string line, int lineNumber)
        {
            line = ExpandTabs(line);

            var groups = new string[0];
            var allMatched = true;
            var lineInfo = new LineInfo(line);
            Block container = document;
            ListData listData;

            while (container.Children.Any())
            {
                var lastChild = container.LastChild;
                if (!lastChild.IsOpen)
                {
                    break;
                }
                container = lastChild;
                lineInfo.FindFirstNonSpace();
                allMatched = container.MatchNextLine(lineInfo) && allMatched;

                if (!allMatched)
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
            if (lineInfo.Blank && container.LastLineIsBlank)
            {
                BreakOutOfLists(document, container, lineNumber);
            }

            // Unless last matched container is a code block, try new container starts,
            // adding children to the last matched container:
            while (!(container is FencedCode || container is IndentedCode || container is HtmlBlock))
            {
                lineInfo.FindFirstNonSpace();

                if (lineInfo.Indent >= CODE_INDENT)
                {
                    // indented code
                    if (!(document.Tip is Paragraph) && !lineInfo.Blank)
                    {
                        lineInfo.Offset += CODE_INDENT;
                        closeUnmatchedBlocks();
                        container = document.AddBlock(this, lineNumber, new IndentedCode { StartLine = lineNumber });
                    }
                    else
                    { // indent > 4 in a lazy paragraph continuation
                        break;
                    }
                }
                else if (lineInfo[lineInfo.FirstNonSpace] == '>')
                {
                    // blockquote
                    lineInfo.Offset = lineInfo.FirstNonSpace + 1;
                    // optional following space
                    if (lineInfo[lineInfo.Offset] == ' ')
                    {
                        lineInfo.Offset++;
                    }
                    closeUnmatchedBlocks();
                    container = document.AddBlock(this, lineNumber, new BlockQuote { StartLine = lineNumber });
                }
                else if (RegexUtils.IsMatch(lineInfo.Line, @"\G(#{1,6})(?: +|$)", lineInfo.FirstNonSpace, out groups))
                {
                    // ATX header
                    lineInfo.Offset = lineInfo.FirstNonSpace + groups[0].Length;
                    closeUnmatchedBlocks();
                    var atxHeaderLevel = groups[1].Length;
                    var atxHeaderText = lineInfo.FromOffset;
                    atxHeaderText = Regex.Replace(atxHeaderText, @"(?:(\\#) *#*| *#+) *$", "$1");

                    container = document.AddBlock(this, lineNumber, new ATXHeader(
                        groups[1].Length,
                        // remove trailing ###s:
                        Regex.Replace(lineInfo.FromOffset, @"(?:(\\#) *#*| *#+) *$", "$1")
                    ) { StartLine = lineNumber });
                    break;
                }
                else if (RegexUtils.IsMatch(lineInfo.Line, @"\G`{3,}(?!.*`)|^~{3,}(?!.*~)", lineInfo.FirstNonSpace, out groups))
                {
                    // fenced code block
                    var fence_length = groups[0].Length;
                    closeUnmatchedBlocks();
                    container = document.AddBlock(this, lineNumber, new FencedCode
                    {
                        StartLine = lineNumber,
                        Length = fence_length,
                        Char = groups[0][0],
                        Offset = lineInfo.FirstNonSpace - lineInfo.Offset
                    });
                    lineInfo.Offset = lineInfo.FirstNonSpace + fence_length;
                    break;
                }
                else if (Patterns.HtmlBlockTagRe.IsMatch(lineInfo.Line, lineInfo.FirstNonSpace, out groups))
                {
                    // html block
                    closeUnmatchedBlocks();
                    container = document.AddBlock(this, lineNumber, new HtmlBlock { StartLine = lineNumber });
                    // note, we don't adjust offset because the tag is part of the text
                    break;
                }
                else if (container is Paragraph && container.Strings.Count() == 1 &&
                    RegexUtils.IsMatch(lineInfo.Line, @"\G(?:=+|-+) *$", lineInfo.FirstNonSpace, out groups))
                {
                    // setext header line
                    closeUnmatchedBlocks();
                    container = container.Parent.Replace(container, new SetextHeader(groups[0][0] == '=' ? 1 : 2, container.Strings.First())
                    {
                        StartLine = lineNumber
                    });
                    lineInfo.Offset = lineInfo.Line.Length;
                }
                else if (Patterns.HRuleRe.IsMatch(lineInfo.Line, lineInfo.FirstNonSpace, out groups))
                {
                    // hrule
                    closeUnmatchedBlocks();
                    container = document.AddBlock(this, lineNumber, new HorizontalRule { StartLine = lineNumber });
                    lineInfo.Offset = lineInfo.Line.Length - 1;
                    break;
                }
                else if ((listData = ParseListMarker(lineInfo)) != null)
                {
                    // list item
                    closeUnmatchedBlocks();
                    listData.MarkerOffset = lineInfo.Indent;
                    lineInfo.Offset = lineInfo.FirstNonSpace + listData.Padding;

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

            lineInfo.FindFirstNonSpace();
            // First check for a lazy paragraph continuation:
            if (document.Tip != lastMatchedContainer && !lineInfo.Blank &&
                document.Tip is Paragraph && document.Tip.Strings.Any())
            {
                // lazy paragraph continuation
                //document.Tip.LastLineIsBlank = false;
                document.Tip.AddLine(lineInfo.FromOffset);
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
                if (!lineInfo.Blank || container is BlockQuote || container is FencedCode)
                {
                    container.LastLineIsBlank = false;
                }
                else if (container is ListItem)
                {
                    container.LastLineIsBlank = container.Children.Any() && container.StartLine < lineNumber;
                }
                else
                {
                    container.LastLineIsBlank = true;
                }
                //container.LastLineIsBlank = lineInfo.Blank &&
                //  !(container is BlockQuote ||
                //    container is FencedCode || (
                //        container is ListItem &&
                //        !container.Children.Any() && container.StartLine == lineNumber
                //    )
                //  );

                var cont = container;
                while (cont.Parent is Block)
                {
                    cont.Parent.LastLineIsBlank = false;
                    cont = cont.Parent;
                }

                if (container is IndentedCode || container is HtmlBlock)
                {
                    document.Tip.AddLine(lineInfo.FromOffset);
                }
                else if (container is FencedCode)
                {
                    var fencedCode = container as FencedCode;
                    // check for closing code fence:
                    var match =
                        lineInfo.Indent <= 3 &&
                        lineInfo[lineInfo.FirstNonSpace] == fencedCode.Char &&
                        RegexUtils.IsMatch(lineInfo.Line, @"\G(?:`{3,}|~{3,})(?= *$)", lineInfo.FirstNonSpace, out groups);

                    if (match && groups[0].Length >= fencedCode.Length)
                    {
                        // don't add closing fence to container; instead, close it:
                        container.Close(this, lineNumber);
                    }
                    else
                    {
                        document.Tip.AddLine(lineInfo.FromOffset);
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
                        document.Tip.AddLine(lineInfo.FromNonSpace);
                    }
                    else if (lineInfo.Blank)
                    {
                        // do nothing
                    }
                    else if (!(container is HorizontalRule || container is SetextHeader))
                    {
                        // create paragraph container for line
                        container = document.AddBlock(this, lineNumber, new Paragraph { StartLine = lineNumber });
                        document.Tip.AddLine(lineInfo.FromNonSpace);
                    }
                    else
                    {
                        throw new Exception(
                            "Line " + // line_number.toString() +
                            " with container type " + container.GetType().Name +
                            " did not match any condition."
                        );
                    }
                }
            }
        }

        private ListData ParseListMarker(LineInfo lineInfo)
        {
            var rest = lineInfo.FromNonSpace;
            var groups = new string[0];
            var spaces_after_marker = 0;
            var data = new ListData();
            if (rest == "" || lineInfo.FirstNonSpace < lineInfo.Line.Length && Patterns.HRuleRe.IsMatch(lineInfo.Line, lineInfo.FirstNonSpace))
            {
                return null;
            }
            if (RegexUtils.IsMatch(rest, @"^[*+-]( +|$)", out groups))
            {
                spaces_after_marker = groups[1].Length;
                data.Type = "Bullet";
                data.BulletChar = groups[0][0];
            }
            else if (RegexUtils.IsMatch(rest, @"^(\d+)([.)])( +|$)", out groups))
            {
                spaces_after_marker = groups[3].Length;
                data.Type = "Ordered";
                data.Start = int.Parse(groups[1]);
                data.Delimiter = groups[2];
            }
            else
            {
                return null;
            }
            var blank_item = groups[0].Length == rest.Length;
            if (spaces_after_marker >= 5 || spaces_after_marker < 1 || blank_item)
            {
                data.Padding = groups[0].Length - spaces_after_marker + 1;
            }
            else
            {
                data.Padding = groups[0].Length;
            }
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

        private string ExpandTabs(string line)
        {
            if (!line.Contains('\t'))
            {
                return line;
            }
            var lastStop = 0;
            return Regex.Replace(line, "\t", m =>
            {
                var result = new string(' ', 4 - (m.Index - lastStop) % 4);
                lastStop = m.Index + 1;
                return result;
            });
        }
    }
}
