using CommonMarkSharp.Inlines;
using System;
using System.Collections.Generic;

namespace CommonMarkSharp.Blocks
{
    public class Document : ContainerBlock
    {
        private Dictionary<string, Link> _linkDefinitions = new Dictionary<string, Link>(StringComparer.InvariantCultureIgnoreCase);

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
