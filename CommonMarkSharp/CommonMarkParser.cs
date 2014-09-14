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
            while (context.SetLine(ExpandTabs(reader.ReadLine())))
            {
                ProcessLine(context.CreateChild());
            }

            while (context.Tip is Block)
            {
                context.Tip.Close(context);
            }

            HandleInlines(context, context.Document);
            //context.Document.Accept(new HandleInlinesVisitor(context));
            return context.Document;
        }

        public virtual void HandleInlines(ParserContext context, Block block)
        {
            var leafBlockWithInlines = block as LeafBlockWithInlines;
            if (leafBlockWithInlines != null)
            {
                leafBlockWithInlines.Inlines = Parsers.CommonMarkInlineParser.ParseMany(context, block.Contents.Trim());
            }
            foreach (var child in block.Children)
            {
                HandleInlines(context, child);
            }
        }

        private void ProcessLine(ParserContext context)
        {
            var groups = new string[0];
            var subject = new Subject(context.Line);

            context.Container = context.Document;
            while (context.Container.LastChild != null && context.Container.LastChild.IsOpen)
            {
                context.Container = context.Container.LastChild;
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
            context.CloseUnmatchedBlocks = () =>
            {
                // finalize any blocks not matched
                while (oldtip != lastMatchedContainer)
                {
                    oldtip.Close(context);
                    oldtip = oldtip.Parent;
                }
                context.CloseUnmatchedBlocks = () => { };
            };

            // Check to see if we've hit 2nd blank line; if so break out of list:
            if (subject.IsBlank && context.Container.LastLineIsBlank)
            {
                BreakOutOfLists(context, context.Container, context.LineNumber);
            }

            // Unless last matched context.Container is a code block, try new context.Container starts,
            // adding children to the last matched context.Container:
            while (!context.Container.IsCode && !context.BlocksParsed)
            {
                var parsed = context.Parsers.IndentedCodeParser.Parse(context, subject);
                parsed = parsed || context.Parsers.LazyParagraphContinuationParser.Parse(context, subject);
                parsed = parsed || context.Parsers.BlockQuoteParser.Parse(context, subject);
                parsed = parsed || context.Parsers.ATXHeaderParser.Parse(context, subject);
                parsed = parsed || context.Parsers.FencedCodeParser.Parse(context, subject);
                parsed = parsed || context.Parsers.HtmlBlockParser.Parse(context, subject);
                parsed = parsed || context.Parsers.SetExtHeaderParser.Parse(context, subject);
                parsed = parsed || context.Parsers.HorizontalRuleParser.Parse(context, subject);
                parsed = parsed || context.Parsers.ListParser.Parse(context, subject);

                if (!parsed || context.Container.AcceptsLines)
                {
                    // if none were found or it's a line context.Container, it can't contain other containers
                    context.BlocksParsed = true;
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
                context.CloseUnmatchedBlocks();

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
                    context.Container.LastLineIsBlank = context.Container.StartLine < context.LineNumber;
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
                        context.Container.Close(context);
                    }
                    else
                    {
                        context.Tip.AddLine(subject.Rest);
                    }
                }
                else if (context.Container is ATXHeader || context.Container is SetExtHeader || context.Container is HorizontalRule)
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
                    else if (!(context.Container is HorizontalRule || context.Container is SetExtHeader))
                    {
                        // create paragraph context.Container for line
                        context.AddBlock(new Paragraph());
                        subject.AdvanceToFirstNonSpace();
                        context.Tip.AddLine(subject.Rest);
                    }
                    else
                    {
                        throw new Exception(
                            "Line " + context.LineNumber.ToString() +
                            " with context.Container type " + context.Container.GetType().Name +
                            " did not match any condition."
                        );
                    }
                }
            }
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
                    block.Close(context);
                    block = block.Parent;
                }
                lastList.Close(context);
            }
        }

        private string ExpandTabs(string line, int tabWidth = 4)
        {
            if (line == null)
            {
                return null;
            }
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
