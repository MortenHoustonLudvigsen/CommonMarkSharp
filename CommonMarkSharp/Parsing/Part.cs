using CommonMarkSharp.Parsing.Blocks;
using CommonMarkSharp.Parsing.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Parsing
{
    public abstract class Part
    {
        //public Part Parent { get; private set; }

        //public Part Root
        //{
        //    get
        //    {
        //        if (Parent == null)
        //        {
        //            return this;
        //        }
        //        return Parent.Root;
        //    }
        //}

        //public Document Document
        //{
        //    get { return Root as Document; }
        //}

        //protected TPart SetParent<TPart>(TPart part)
        //    where TPart : Part
        //{
        //    part.Parent = this;
        //    return part;
        //}

        //public bool HasParent<TPart>()
        //    where TPart: Part
        //{
        //    if (Parent == null)
        //    {
        //        return false;
        //    }
        //    if (Parent is TPart)
        //    {
        //        return true;
        //    }
        //    return Parent.HasParent<TPart>();
        //}

        //private readonly List<Part> _children = new List<Part>();
        //public IEnumerable<Part> Children { get { return _children; } }

        //public void Add(params Part[] parts)
        //{
        //    Add(parts.AsEnumerable());
        //}

        //public void Add(IEnumerable<Part> parts)
        //{
        //    foreach (var part in parts)
        //    {
        //        AddChild(SetParent(part));
        //    }
        //}

        //protected virtual void AddChild(Part part)
        //{
        //    _children.Add(part);
        //}

        //public TPart Replace<TPart>(Part originalPart, TPart newPart)
        //    where TPart : Part
        //{
        //    var index = _children.IndexOf(originalPart);
        //    if (index >= 0)
        //    {
        //        _children[index] = SetParent(newPart);
        //    }
        //    else
        //    {
        //        Add(newPart);
        //    }
        //    return newPart;
        //}

        //public bool Remove(Part part)
        //{
        //    return _children.Remove(part);
        //}

        public virtual void Accept(CommonMarkVisitor visitor)
        {
            visitor.VisitPart(this);
        }
    }
}
