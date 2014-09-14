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
        }

        private ParserContext(ParserContext context)
        {
            Document = context.Document;
            foreach (var param in context._params)
            {
                _params.Add(param.Key, param);
            }
        }

        public Parsers Parsers { get; private set; }
        public Document Document { get; private set; }
        public Block Container { get; set; }
        public Block Tip { get; set; }

        public TBlock AddBlock<TBlock>(int lineNumber, TBlock block)
            where TBlock : Block
        {
            while (!Tip.CanContain(block))
            {
                Tip.Close(this, lineNumber);
            }
            Tip.Add(block);
            Tip = block;
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

        public ParserContext Add(object param, object value = null)
        {
            var context = new ParserContext(this);
            context._params[param] = value;
            return context;
        }
    }
}
