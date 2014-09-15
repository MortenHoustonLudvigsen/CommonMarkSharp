using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace CommonMarkSharp
{
    public abstract class CommonMarkVisitor
    {
        private readonly Dictionary<Type, VisitorDelegate> _visitors;

        public CommonMarkVisitor()
        {
            _visitors = VisitorInfo.GetVisitors(this, "Visit");
        }

        private Stack<Part> _currents = new Stack<Part>();
        public Part Current { get { return _currents.Count > 0 ? _currents.Peek() : null; } }

        public void Add<TPart>(Action<TPart> visitor)
            where TPart : Part
        {
            _visitors[typeof(TPart)] = CreateDelegate(visitor.Target, visitor.Method);
        }

        private VisitorDelegate FindVisitor(Type partType)
        {
            VisitorDelegate visitor = null;
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

        public static bool Lal(int c)
        {
            if (c == 5)
            {
                return true;
            }
            return false;
        }

        public static bool Slam(char c)
        {
            switch (c)
            {
                case 'a':
                case 'b':
                case 'c':
                case 's':
                case 't':
                case 'A':
                case 'B':
                case 'C':
                case 'S':
                case 'T':
                    return true;
                default:
                    return false;
            }
        }

        public static bool Slam(int c)
        {
            switch (c)
            {
                case 3:
                case 4:
                case 5:
                case 8:
                case 9:
                    return true;
                default:
                    return false;
            }
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
                    visitor(part);
                }
                finally
                {
                    _currents.Pop();
                }
            }
        }

        private delegate void VisitorDelegate(Part part);
        private static VisitorDelegate CreateDelegate(object target, MethodInfo method)
        {
            var dynam = new DynamicMethod(
                method.Name,
                typeof(void),
                new[] { typeof(object), typeof(Part) },
                method.DeclaringType
            );

            var il = dynam.GetILGenerator();

            // Argument 0 of dynamic method is target instance.
            il.Emit(OpCodes.Ldarg_0);

            // Argument 1 of dynamic method is the part.
            il.Emit(OpCodes.Ldarg_1);

            // Call method
            il.Emit(OpCodes.Callvirt, method);

            // Emit return opcode.
            il.Emit(OpCodes.Ret);

            return (VisitorDelegate)dynam.CreateDelegate(typeof(VisitorDelegate), target);
        }

        private class VisitorInfo
        {
            public static Dictionary<Type, VisitorDelegate> GetVisitors(CommonMarkVisitor visitor, string name)
            {
                return visitor
                    .GetType()
                    .GetMethods()
                    .Where(m => m.Name == name)
                    .Select(m => new VisitorInfo(m))
                    .Where(m => m.IsValid)
                    .ToDictionary(m => m.PartType, m => CreateDelegate(visitor, m.Method));
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
    }
}
