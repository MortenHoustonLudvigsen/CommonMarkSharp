using CommonMarkSharp.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Blocks
{
    public class Document : ContainerBlock
    {
        public Document()
        {
            //Tip = this;
        }

        private Dictionary<string, Link> _linkDefinitions = new Dictionary<string, Link>(StringComparer.InvariantCultureIgnoreCase);

        //public Block Tip { get; set; }

        //public TBlock AddBlock<TBlock>(CommonMarkParser parser, int lineNumber, TBlock block)
        //    where TBlock : Block
        //{
        //    while (!Tip.CanContain(block))
        //    {
        //        Tip.Close(parser, lineNumber);
        //    }
        //    Tip.Add(block);
        //    Tip = block;
        //    return block;
        //}

        public void AddLinkDefinition(LinkDefinition linkDefinition)
        {
            if (!_linkDefinitions.ContainsKey(linkDefinition.Id))
            {
                _linkDefinitions[linkDefinition.Id] = linkDefinition.Link;
            }
        }

        public override bool CanContain(Block block)
        {
            return true;
        }

        public Link FindLink(string id)
        {
            Link linkDefinition;
            if (_linkDefinitions.TryGetValue(id, out linkDefinition))
            {
                return linkDefinition;
            }
            return null;
        }
    }
}
