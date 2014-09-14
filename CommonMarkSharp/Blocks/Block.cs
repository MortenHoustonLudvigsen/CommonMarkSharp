using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonMarkSharp.Blocks
{
    public abstract class Block : Part
    {
        private readonly List<string> _strings = new List<string>();

        public Block()
        {
            IsOpen = true;
            Parent = null;
            Contents = "";
        }

        private Block _parent;
        public Block Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                Root = Parent == null ? this : Parent.Root;
            }
        }

        public Block Root { get; private set; }
        public Document Document { get { return Root as Document; } }

        private int _startLine;
        public int StartLine
        {
            get { return _startLine; }
            set { _startLine = value; EndLine = value; }
        }
        public int EndLine { get; set; }

        protected TBlock SetParent<TBlock>(TBlock block)
            where TBlock : Block
        {
            block.Parent = this;
            return block;
        }

        private readonly List<Block> _children = new List<Block>();
        public IEnumerable<Block> Children { get { return _children; } }

        public void Add(params Block[] blocks)
        {
            Add(blocks.AsEnumerable());
        }

        public void Add(IEnumerable<Block> blocks)
        {
            foreach (var part in blocks)
            {
                AddChild(part);
            }
        }

        protected virtual void AddChild(Block block)
        {
            _children.Add(SetParent(block));
            LastChild = block;
        }

        public TBlock Replace<TBlock>(ParserContext context, Block originalBlock, TBlock newBlock)
            where TBlock : Block
        {
            var index = _children.IndexOf(originalBlock);
            if (index >= 0)
            {
                _children[index] = SetParent(newBlock);
            }
            else
            {
                Add(newBlock);
            }
            LastChild = _children.LastOrDefault();

            if (context.Tip == originalBlock)
            {
                context.Tip = newBlock;
            }

            return newBlock;
        }

        public bool Remove(Block block)
        {
            var result = _children.Remove(block);
            LastChild = _children.LastOrDefault();
            return result;
        }

        public virtual bool AcceptsLines { get { return false; } }
        public virtual bool IsCode { get { return false; } }
        public bool IsOpen { get; protected set; }
        public IEnumerable<string> Strings { get { return _strings; } }
        public string Contents { get; set; }
        public bool LastLineIsBlank { get; set; }
        public Block LastChild { get; private set; }

        public virtual void Close(ParserContext context)
        {
            if (!IsOpen)
            {
                throw new Exception("This block is already closed");
            }
            IsOpen = false;
            EndLine = context.LineNumber;
            context.Tip = context.Tip.Parent;
        }

        public virtual bool MatchNextLine(Subject subject)
        {
            return true;
        }

        public virtual bool CanContain(Block block)
        {
            return false;
        }

        public void AddLine(string line)
        {
            if (!AcceptsLines)
            {
                throw new Exception("This block does not accept lines");
            }
            _strings.Add(line);
        }

        // Returns true if block ends with a blank line, descending if needed
        // into lists and sublists.
        public virtual bool EndsWithBlankLine
        {
            get { return LastLineIsBlank; }
        }
    }
}
