using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp
{
    public static class SubjectExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountWhile(this Subject subject, char c, long max = int.MaxValue)
        {
            var start = subject.Index;
            var index = start;
            max = Math.Min(start + max, subject.Text.Length);
            while (index < max && subject.Text[index] == c)
            {
                index += 1;
            }
            return index - start;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountWhileNot(this Subject subject, char c, long max = int.MaxValue)
        {
            var start = subject.Index;
            var index = start;
            max = Math.Min(start + max, subject.Text.Length);
            while (index < max && subject.Text[index] != c)
            {
                index += 1;
            }
            return index - start;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountWhile(this Subject subject, CharSet chars, long max = int.MaxValue)
        {
            var start = subject.Index;
            var index = start;
            max = Math.Min(start + max, subject.Text.Length);
            while (index < max && chars.Contains(subject.Text[index]))
            {
                index += 1;
            }
            return index - start;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountWhileNot(this Subject subject, CharSet chars, long max = int.MaxValue)
        {
            var start = subject.Index;
            var index = start;
            max = Math.Min(start + max, subject.Text.Length);
            while (index < max && !chars.Contains(subject.Text[index]))
            {
                index += 1;
            }
            return index - start;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountWhile(this Subject subject, Func<char, bool> predicate, long max = int.MaxValue)
        {
            var start = subject.Index;
            var index = start;
            max = Math.Min(start + max, subject.Text.Length);
            while (index < max && predicate(subject.Text[index]))
            {
                index += 1;
            }
            return index - start;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountWhileNot(this Subject subject, Func<char, bool> predicate, long max = int.MaxValue)
        {
            var start = subject.Index;
            var index = start;
            max = Math.Min(start + max, subject.Text.Length);
            while (index < max && !predicate(subject.Text[index]))
            {
                index += 1;
            }
            return index - start;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AdvanceWhile(this Subject subject, char c, long max = int.MaxValue)
        {
            var start = subject.Index;
            var index = start;
            max = Math.Min(start + max, subject.Text.Length);
            while (index < max && subject.Text[index] == c)
            {
                index += 1;
            }
            return index > start ? subject.Advance(index - start) : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AdvanceWhileNot(this Subject subject, char c, long max = int.MaxValue)
        {
            var start = subject.Index;
            var index = start;
            max = Math.Min(start + max, subject.Text.Length);
            while (index < max && subject.Text[index] != c)
            {
                index += 1;
            }
            return index > start ? subject.Advance(index - start) : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AdvanceWhile(this Subject subject, Func<char, bool> predicate, long max = int.MaxValue)
        {
            var start = subject.Index;
            var index = start;
            max = Math.Min(start + max, subject.Text.Length);
            while (index < max && predicate(subject.Text[index]))
            {
                index += 1;
            }
            return index > start ? subject.Advance(index - start) : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AdvanceWhileNot(this Subject subject, Func<char, bool> predicate, long max = int.MaxValue)
        {
            var start = subject.Index;
            var index = start;
            max = Math.Min(start + max, subject.Text.Length);
            while (index < max && !predicate(subject.Text[index]))
            {
                index += 1;
            }
            return index > start ? subject.Advance(index - start) : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char Take(this Subject subject)
        {
            if (subject.EndOfString)
            {
                return char.MinValue;
            }
            var result = subject.Char;
            subject.Advance();
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Take(this Subject subject, int count)
        {
            var result = subject.Text.Substring(subject.Index, count);
            subject.Advance(count);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string TakeWhile(this Subject subject, char c, long max = int.MaxValue)
        {
            var start = subject.Index;
            var index = start;
            max = Math.Min(start + max, subject.Text.Length);
            while (index < max && subject.Text[index] == c)
            {
                index += 1;
            }
            return index > start ? subject.Take(index - start) : "";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string TakeWhileNot(this Subject subject, char c, long max = int.MaxValue)
        {
            var start = subject.Index;
            var index = start;
            max = Math.Min(start + max, subject.Text.Length);
            while (index < max && subject.Text[index] != c)
            {
                index += 1;
            }
            return index > start ? subject.Take(index - start) : "";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string TakeWhile(this Subject subject, CharSet chars, long max = int.MaxValue)
        {
            var start = subject.Index;
            var index = start;
            max = Math.Min(start + max, subject.Text.Length);
            while (index < max && chars.Contains(subject.Text[index]))
            {
                index += 1;
            }
            return index > start ? subject.Take(index - start) : "";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string TakeWhileNot(this Subject subject, CharSet chars, long max = int.MaxValue)
        {
            var start = subject.Index;
            var index = start;
            max = Math.Min(start + max, subject.Text.Length);
            while (index < max && !chars.Contains(subject.Text[index]))
            {
                index += 1;
            }
            return index > start ? subject.Take(index - start) : "";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string TakeWhile(this Subject subject, Func<char, bool> predicate, long max = int.MaxValue)
        {
            var start = subject.Index;
            var index = start;
            max = Math.Min(start + max, subject.Text.Length);
            while (index < max && predicate(subject.Text[index]))
            {
                index += 1;
            }
            return index > start ? subject.Take(index - start) : "";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string TakeWhileNot(this Subject subject, Func<char, bool> predicate, long max = int.MaxValue)
        {
            var start = subject.Index;
            var index = start;
            max = Math.Min(start + max, subject.Text.Length);
            while (index < max && !predicate(subject.Text[index]))
            {
                index += 1;
            }
            return index > start ? subject.Take(index - start) : "";
        }
    }
}
