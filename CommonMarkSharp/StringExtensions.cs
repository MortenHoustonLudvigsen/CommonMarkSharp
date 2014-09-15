using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp
{
    public static class StringExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StartsWith(this string str, int start, string value)
        {
            if (value.Length > str.Length - start)
            {
                return false;
            }
            var index = 0;
            while (index < value.Length && value[index] == str[index + start])
            {
                index += 1;
            }
            return index == value.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountWhile(this string str, int start, char c, long max = int.MaxValue)
        {
            var index = start;
            max = Math.Min(index + max, str.Length);
            while (index < max && str[index] == c)
            {
                index += 1;
            }
            return index - start;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountWhileNot(this string str, int start, char c, long max = int.MaxValue)
        {
            var index = start;
            max = Math.Min(index + max, str.Length);
            while (index < max && str[index] != c)
            {
                index += 1;
            }
            return index - start;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountWhile(this string str, int start, CharSet chars, long max = int.MaxValue)
        {
            var index = start;
            max = Math.Min(index + max, str.Length);
            while (index < max && chars.Contains(str[start]))
            {
                index += 1;
            }
            return index - start;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountWhileNot(this string str, int start, CharSet chars, long max = int.MaxValue)
        {
            var index = start;
            max = Math.Min(index + max, str.Length);
            while (index < max && !chars.Contains(str[start]))
            {
                index += 1;
            }
            return index - start;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountWhile(this string str, int start, Func<char, bool> predicate, long max = int.MaxValue)
        {
            var index = start;
            max = Math.Min(index + max, str.Length);
            while (index < max && predicate(str[start]))
            {
                index += 1;
            }
            return index - start;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountWhileNot(this string str, int start, Func<char, bool> predicate, long max = int.MaxValue)
        {
            var index = start;
            max = Math.Min(index + max, str.Length);
            while (index < max && !predicate(str[start]))
            {
                index += 1;
            }
            return index - start;
        }
    }
}
