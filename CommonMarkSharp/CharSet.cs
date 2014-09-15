using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp
{
    public struct CharSet
    {
        public delegate bool ContainsDeletage(char c);

        public CharSet(string chars)
        {
            Any = chars.Any();
            Chars = OrderChars(chars);
            Contains = Create(chars);
        }

        public CharSet(IEnumerable<char> chars)
            : this(new string(chars.ToArray()))
        {
        }

        public CharSet(CharSet chars)
            : this(chars.Chars)
        {
        }

        public readonly string Chars;
        public readonly bool Any;
        public readonly ContainsDeletage Contains;

        public static implicit operator CharSet(string chars)
        {
            return new CharSet(chars);
        }

        public static CharSet operator +(CharSet c1, CharSet c2)
        {
            return new CharSet(c1.Chars + c2.Chars);
        }

        public static CharSet operator +(CharSet c1, string c2)
        {
            return new CharSet(c1.Chars + c2);
        }

        public static CharSet operator +(string c1, CharSet c2)
        {
            return new CharSet(c1 + c2.Chars);
        }

        public static CharSet operator -(CharSet c1, CharSet c2)
        {
            return new CharSet(c1.Chars.Where(c => !c2.Contains(c)));
        }

        public static CharSet operator -(CharSet c1, string c2)
        {
            return new CharSet(c1.Chars.Where(c => !c2.Contains(c)));
        }

        public static CharSet operator -(string c1, CharSet c2)
        {
            return new CharSet(c1.Where(c => !c2.Contains(c)));
        }

        private static ContainsDeletage Create(string chars)
        {
            var dynam = new DynamicMethod(
                "Contains",
                typeof(bool),
                new[] { typeof(char) },
                typeof(CharSet)
            );

            var il = dynam.GetILGenerator();
            var foundLabel = il.DefineLabel();
            var notFoundLabel = il.DefineLabel();

            foreach (var range in GetRanges(chars))
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldc_I4, range.Start);
                il.Emit(OpCodes.Blt, notFoundLabel);
                if (range.Length == 1)
                {
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldc_I4, range.Start);
                    il.Emit(OpCodes.Beq, foundLabel);
                }
                else
                {
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldc_I4, range.End);
                    il.Emit(OpCodes.Ble, foundLabel);
                }
            }

            il.MarkLabel(notFoundLabel);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Ret);

            il.MarkLabel(foundLabel);
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Ret);

            return (ContainsDeletage)dynam.CreateDelegate(typeof(ContainsDeletage));
        }

        private class Range
        {
            public Range(int c)
            {
                Start = c;
                End = c;
            }

            public int Start;
            public int End;
            public int Length { get { return End - Start + 1; } }
        }

        private static string OrderChars(string chars)
        {
            return new string(chars.Distinct().OrderBy(c => c).ToArray());
        }

        private static IEnumerable<Range> GetRanges(string chars)
        {
            var orderedChars = OrderChars(chars).Select(c => (int)c);
            var count = orderedChars.Count();
            Range range = null;

            foreach (var c in orderedChars)
            {
                if (range == null)
                {
                    range = new Range(c);
                }
                else if (range.End + 1 < c)
                {
                    yield return range;
                    range = new Range(c);
                }
                else
                {
                    range.End = c;
                }
            }
            if (range != null)
            {
                yield return range;
            }
        }
    }
}
