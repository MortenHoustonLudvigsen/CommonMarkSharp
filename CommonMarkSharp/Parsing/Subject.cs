using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonMarkSharp.Parsing
{
    public class Subject
    {
        public const string WhiteSpace = " \t\n";

        public Subject(string text)
        {
            if (text == null) throw new ArgumentNullException("text");
            Text = text;
            Index = 0;
        }

        public string Text { get; private set; }
        public int Index { get; private set; }
        public char Char { get { return GetChar(Index); } }
        public char Previous { get { return GetChar(Index - 1); } }
        public bool EndOfString { get { return Index >= Text.Length; } }
        public string Rest { get { return EndOfString ? "" : Text.Substring(Index); } }

        public char this[int relativeIndex]
        {
            get
            {
                var index = Index + relativeIndex;
                if (index >= 0 && index < Text.Length)
                {
                    return Text[index];
                }
                return char.MinValue;
            }
        }

        public void Advance(int count = 1)
        {
            Index += count;
        }

        public void Rewind(int count)
        {
            Index -= count;
            if (Index < 0)
            {
                throw new Exception("Rewound too far");
            }
        }

        public SavedSubject Save()
        {
            return new SavedSubject(this);
        }

        public int AdvanceWhile(Func<char, bool> predicate, int max = int.MaxValue)
        {
            var advanced = 0;
            while (!EndOfString && advanced < max && predicate(Char))
            {
                advanced += 1;
                Advance();
            }
            return advanced;
        }

        public int SkipWhiteSpace()
        {
            return AdvanceWhile(c => IsWhiteSpace(c));
        }

        public static bool IsWhiteSpace(char c)
        {
            return WhiteSpace.Contains(c);
        }

        public bool IsWhiteSpace()
        {
            return IsWhiteSpace(Char);
        }

        public bool IsWhiteSpace(int relativeIndex)
        {
            return IsWhiteSpace(this[relativeIndex]);
        }

        public char Take()
        {
            var result = Char;
            Advance();
            return result;
        }

        public IEnumerable<char> Take(int count)
        {
            var chars = new char[count];
            for (var i = 0; i < count && !EndOfString; i++)
            {
                chars[i] = Take();
            }
            return chars;
        }

        public IEnumerable<char> TakeWhile(Func<char, bool> predicate)
        {
            return TakeWhile(() => predicate(Char));
        }

        public IEnumerable<char> TakeWhile(Func<bool> predicate)
        {
            var chars = "";
            while (!EndOfString && predicate())
            {
                chars += Take();
            }
            return chars;
        }

        public int CountWhile(Func<char, bool> predicate)
        {
            var count = 0;
            while (!EndOfString && predicate(this[count]))
            {
                count += 1;
            }
            return count;
        }

        public bool StartsWith(string str, int relativeIndex)
        {
            var index = Index + relativeIndex;
            if (index >= 0 && index < Text.Length)
            {
                return Text.Substring(index).StartsWith(str);
            }
            return false;
        }

        public bool IsMatch(string pattern, int relativeIndex)
        {
            string[] groups;
            return IsMatch(pattern, relativeIndex, out groups);
        }

        public bool IsMatch(string pattern, int relativeIndex, out string[] groups)
        {
            return IsMatch(new Regex(@"\G" + pattern), relativeIndex, out groups);
        }

        public bool IsMatch(Regex re, int relativeIndex)
        {
            string[] groups;
            return IsMatch(re, relativeIndex, out groups);
        }

        public bool IsMatch(Regex re, int relativeIndex, out string[] groups)
        {
            return re.IsMatch(Text, Index + relativeIndex, out groups);
        }

        public bool PartOfSequence(char c, int count)
        {
            var savedSubject = Save();
            while (Char == c)
            {
                Advance(-1);
            }
            Advance();
            while (count > 0 && Char == c)
            {
                count -= 1;
                Advance(1);
            }
            savedSubject.Restore();
            return count <= 0;
        }

        private char GetChar(int index)
        {
            return index >= 0 && index < Text.Length ? Text[index] : (char)0;
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
                .Replace("\v", "\\v")
                ;
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
        }
    }
}
