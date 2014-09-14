using CommonMarkSharp.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp
{
    public class ParserContext
    {
        private Dictionary<object, object> _params = new Dictionary<object, object>();

        public ParserContext(Parsers parsers, Document document)
        {
            Parsers = parsers;
            Document = document;
            Container = document;
            Tip = document;
            LineNumber = 0;
            CloseUnmatchedBlocks = () => { };
        }

        private ParserContext()
        {
        }

        public virtual Parsers Parsers { get; private set; }
        public virtual Document Document { get; private set; }
        public virtual int LineNumber { get; private set; }
        public virtual string Line { get; private set; }
        public virtual Block Container { get; set; }
        public virtual Block Tip { get; set; }
        public bool BlocksParsed { get; set; }
        public Action CloseUnmatchedBlocks { get; set; }

        public virtual bool SetLine(string line)
        {
            if (line != null)
            {
                LineNumber += 1;
                Line = line;
                return true;
            }
            return false;
        }

        public TBlock AddBlock<TBlock>(TBlock block)
            where TBlock : Block
        {
            CloseUnmatchedBlocks();
            block.StartLine = LineNumber;
            while (!Tip.CanContain(block))
            {
                Tip.Close(this);
            }
            Tip.Add(block);
            Tip = block;
            Container = block;
            return block;
        }

        public object this[object param]
        {
            get
            {
                object value;
                if (_params.TryGetValue(param, out value))
                {
                    return value;
                }
                return null;
            }
        }

        public bool HasParam(object param)
        {
            return _params.ContainsKey(param);
        }

        public ParserContext CreateChild()
        {
            return new ChildParserContext(this);
        }

        public ParserContext Add(object param, object value = null)
        {
            var context = CreateChild();
            context._params[param] = value;
            return context;
        }

        private class ChildParserContext : ParserContext
        {
            public ChildParserContext(ParserContext parent)
            {
                _parent = parent;
                BlocksParsed = parent.BlocksParsed;
                foreach (var param in parent._params)
                {
                    _params.Add(param.Key, param);
                }
            }

            private ParserContext _parent;

            public override Parsers Parsers { get { return _parent.Parsers; } }
            public override Document Document { get { return _parent.Document; } }
            public override string Line { get { return _parent.Line; } }
            public override int LineNumber { get { return _parent.LineNumber; } }

            public override Block Container
            {
                get { return _parent.Container; }
                set { _parent.Container = value; }
            }

            public override Block Tip
            {
                get { return _parent.Tip; }
                set { _parent.Tip = value; }
            }
        }
    }
}
