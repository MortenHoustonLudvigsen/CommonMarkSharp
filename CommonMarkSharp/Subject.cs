using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonMarkSharp
{
    public class Subject
    {
        private static readonly string[] _emptyGroups = new string[0];

        public Subject(string text)
        {
            Text = text ?? "";
            Index = 0;
        }

        public string Text { get; private set; }

        private int _index;
        public int Index
        {
            get { return _index; }
            private set
            {
                if (value < 0) throw new ArgumentException("Index must not be less than 0");
                if (value > Text.Length) throw new ArgumentException("Index must not be greater than the text length");

                if (value >= Text.Length)
                {
                    _index = Text.Length;
                    EndOfString = true;
                    Char = char.MinValue;
                }
                else
                {
                    _index = value;
                    EndOfString = false;
                    Char = Text[_index];
                }
            }
        }

        public char Char { get; private set; }
        public bool EndOfString { get; private set; }
        public string Rest { get { return EndOfString ? "" : Text.Substring(Index); } }

        public char this[int relativeIndex]
        {
            get { return GetChar(Index + relativeIndex); }
        }

        private bool ValidIndex(int index)
        {
            return index >= 0 && index < Text.Length;
        }

        private char GetChar(int index)
        {
            return ValidIndex(index) ? Text[index] : char.MinValue;
        }

        public SavedSubject Save()
        {
            return new SavedSubject(this);
        }

        public int Advance(int count = 1)
        {
            var originalIndex = Index;
            Index += count;
            return Index - originalIndex;
        }

        public int AdvanceWhile(Func<char, bool> predicate, int max = int.MaxValue)
        {
            return Advance(CountWhile(predicate, max));
        }

        public int Advance(Regex re)
        {
            var groups = _emptyGroups;
            if (IsMatch(re, out groups))
            {
                return Advance(groups[0].Length);
            }
            return 0;
        }

        public int Advance(Regex re, int relativeIndex)
        {
            var groups = _emptyGroups;
            if (IsMatch(re, relativeIndex, out groups))
            {
                return Advance(groups[0].Length);
            }
            return 0;
        }

        public char Take()
        {
            if (EndOfString)
            {
                return char.MinValue;
            }
            var result = Char;
            Advance();
            return result;
        }

        public string Take(int count)
        {
            var result = Text.Substring(Index, count);
            Advance(count);
            return result;
        }

        public string TakeWhile(Func<char, bool> predicate, int max = int.MaxValue)
        {
            return Take(CountWhile(predicate, max));
        }

        public string Take(Regex re)
        {
            var groups = _emptyGroups;
            return Take(re, out groups);
        }

        public string Take(Regex re, int relativeIndex)
        {
            var groups = _emptyGroups;
            return Take(re, out groups);
        }

        public string Take(Regex re, out string[] groups)
        {
            if (IsMatch(re, out groups))
            {
                Advance(groups[0].Length);
                return groups[0];
            }
            return "";
        }

        public string Take(Regex re, int relativeIndex, out string[] groups)
        {
            if (IsMatch(re, relativeIndex, out groups))
            {
                Advance(groups[0].Length);
                return groups[0];
            }
            return "";
        }

        public int CountWhile(Func<char, bool> predicate, int max = int.MaxValue)
        {
            var count = 0;
            var index = Index;
            while (index < Text.Length && count < max && predicate(Text[index]))
            {
                index += 1;
                count += 1;
            }
            return count;
        }

        public bool StartsWith(string str, int relativeIndex = 0)
        {
            var index = Index + relativeIndex;
            if (index + str.Length > Text.Length)
            {
                return false;
            }

            for (var i = 0; i < str.Length; index += 1, i += 1)
            {
                if (str[i] != Text[index])
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsMatch(Regex re)
        {
            return re.IsMatch(Text, Index);
        }

        public bool IsMatch(Regex re, int relativeIndex)
        {
            var index = Index + relativeIndex;
            return ValidIndex(index) ? re.IsMatch(Text, index) : false;
        }

        public bool IsMatch(Regex re, out string[] groups)
        {
            return re.IsMatch(Text, Index, out groups);
        }

        public bool IsMatch(Regex re, int relativeIndex, out string[] groups)
        {
            var index = Index + relativeIndex;
            groups = _emptyGroups;
            return ValidIndex(index) ? re.IsMatch(Text, index, out groups) : false;
        }

        public bool PartOfSequence(char c, int count)
        {
            var index = Index;
            while (index > 0 && Text[index] == c)
            {
                index -= 1;
            }
            index += 1;
            while (index < Text.Length && count > 0 && Text[index] == c)
            {
                count -= 1;
                index += 1;
            }
            return count <= 0;
        }

        public int SkipWhiteSpace()
        {
            return AdvanceWhile(c => IsWhiteSpace(c));
        }

        public static bool IsWhiteSpace(char c)
        {
            return c == ' ' || c == '\t' || c == '\n';
        }

        public bool IsWhiteSpace()
        {
            return IsWhiteSpace(Char);
        }

        public bool IsWhiteSpace(int relativeIndex)
        {
            return IsWhiteSpace(this[relativeIndex]);
        }

        public override string ToString()
        {
            return string.Format("{0}↑{1}", Escape(Text.Substring(0, Index)), Escape(Text.Substring(Index)));
        }

        public static string Escape(string input)
        {
            return input == null ? null : input
                .Replace("\0", "\\0")
                .Replace("\a", "\\a")
                .Replace("\b", "\\b")
                .Replace("\f", "\\f")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t")
                .Replace("\v", "\\v");
        }

        public class SavedSubject
        {
            public SavedSubject(Subject subject)
            {
                Subject = subject;
                Index = Subject.Index;
            }

            public Subject Subject { get; private set; }
            public int Index { get; private set; }

            public void Restore()
            {
                Subject.Index = Index;
            }

            public string GetLiteral()
            {
                return Subject.Text.Substring(Index, Subject.Index - Index);
            }

            public override string ToString()
            {
                return string.Format("{0}↑{1}↑{2}", Escape(Subject.Text.Substring(0, Index)), Escape(Subject.Text.Substring(Index, Subject.Index - Index)), Escape(Subject.Text.Substring(Subject.Index)));
            }
        }
    }
}
