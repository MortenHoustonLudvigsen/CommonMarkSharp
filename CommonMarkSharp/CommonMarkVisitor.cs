using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommonMarkSharp
{
    public abstract class CommonMarkVisitor
    {
        private readonly Dictionary<Type, Visitor> _visitors;

        public CommonMarkVisitor()
        {
            _visitors = VisitorInfo.GetVisitors(this, "Visit");
        }

        private Stack<Part> _currents = new Stack<Part>();
        public Part Current { get { return _currents.Count > 0 ? _currents.Peek() : null; } }

        public void Add<TPart>(Action<TPart> visitor)
            where TPart : Part
        {
            _visitors[typeof(TPart)] = new Visitor(visitor.Target, visitor.Method);
        }

        private Visitor FindVisitor(Type partType)
        {
            Visitor visitor = null;
            if (!_visitors.TryGetValue(partType, out visitor))
            {
                if (partType != typeof(Part) && partType != typeof(object))
                {
                    visitor = FindVisitor(partType.BaseType);
                    _visitors.Add(partType, visitor);
                }
            }
            return visitor;
        }

        public virtual void VisitPart(Part part)
        {
            if (part == null) throw new ArgumentNullException("part");
            var visitor = FindVisitor(part.GetType());
            if (visitor != null)
            {
                _currents.Push(part);
                try
                {
                    visitor.Visit(part);
                }
                finally
                {
                    _currents.Pop();
                }
            }
        }

        private class VisitorInfo
        {
            public static Dictionary<Type, Visitor> GetVisitors(CommonMarkVisitor visitor, string name)
            {
                return visitor
                    .GetType()
                    .GetMethods()
                    .Where(m => m.Name == name)
                    .Select(m => new VisitorInfo(m))
                    .Where(m => m.IsValid)
                    .ToDictionary(m => m.PartType, m => new Visitor(visitor, m.Method));
            }

            public VisitorInfo(MethodInfo method)
            {
                Method = method;
                Parameters = Method.GetParameters();
            }

            public MethodInfo Method { get; private set; }
            public ParameterInfo[] Parameters { get; private set; }
            public bool IsValid { get { return Parameters.Length == 1 && HasParameter<Part>(0); } }
            public Type PartType { get { return Parameters[0].ParameterType; } }

            public bool HasParameter<T>(int index)
            {
                if (index >= Parameters.Length)
                {
                    return false;
                }
                return typeof(T).IsAssignableFrom(Parameters[index].ParameterType);
            }
        }

        private class Visitor
        {
            public Visitor(object target, MethodInfo method)
            {
                Target = target;
                Method = method;
            }

            public object Target { get; private set; }
            public MethodInfo Method { get; private set; }
            public void Visit(Part part)
            {
                Method.Invoke(Target, new[] { part });
            }
        }
    }
}
