using System;

namespace CommonMarkSharp
{
    public class Subject
    {
        private static readonly string[] _emptyGroups = new string[0];

        public Subject(string text)
        {
            Text = text ?? "";
            _length = Text.Length;
            Index = 0;
        }

        public string Text { get; private set; }

        private int _length;
        private int _index;
        public int Index
        {
            get { return _index; }
            private set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Index must not be less than 0");
                }

                if (value >= _length)
                {
                    _index = _length;
                    EndOfString = true;
                    Char = char.MinValue;
                }
                else
                {
                    _index = value;
                    EndOfString = false;
                    Char = Text[_index];
                }
                _firstNonSpace = null;
            }
        }

        public char Char { get; private set; }

        private int? _firstNonSpace;
        public int FirstNonSpace
        {
            get
            {
                if (_firstNonSpace == null)
                {
                    _firstNonSpace = _index + CountWhile(c => c == ' ');
                }
                return _firstNonSpace.Value;
            }
        }

        public char FirstNonSpaceChar { get { return GetChar(FirstNonSpace); } }
        public bool IsBlank { get { return FirstNonSpace == _length; } }
        public int Indent { get { return FirstNonSpace - _index; } }
        public bool EndOfString { get; private set; }
        public string Rest { get { return EndOfString ? "" : Text.Substring(_index); } }

        public char this[int relativeIndex]
        {
            get { return GetChar(_index + relativeIndex); }
        }

        private bool ValidIndex(int index)
        {
            return index >= 0 && index < _length;
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
            var originalIndex = _index;
            Index = _index + count;
            return _index - originalIndex;
        }

        public int AdvanceWhile(Func<char, bool> predicate)
        {
            return Advance(CountWhile(predicate));
        }

        public int AdvanceWhile(Func<char, bool> predicate, int max)
        {
            return Advance(CountWhile(predicate, max));
        }

        public void AdvanceToFirstNonSpace(int offset = 0)
        {
            Index = FirstNonSpace + offset;
        }

        public void AdvanceToEnd(int offset = 0)
        {
            Index = _length + offset;
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
            var result = Text.Substring(_index, count);
            Advance(count);
            return result;
        }

        public string TakeWhile(Func<char, bool> predicate)
        {
            return Take(CountWhile(predicate));
        }

        public string TakeWhile(Func<char, bool> predicate, int max)
        {
            return Take(CountWhile(predicate, max));
        }

        public int CountWhile(Func<char, bool> predicate)
        {
            var count = 0;
            var index = _index;
            while (index < _length && predicate(Text[index]))
            {
                index += 1;
                count += 1;
            }
            return count;
        }

        public int CountWhile(Func<char, bool> predicate, int max)
        {
            var count = 0;
            var index = _index;
            while (index < _length && count < max && predicate(Text[index]))
            {
                index += 1;
                count += 1;
            }
            return count;
        }

        public bool Contains(char c, int relativeIndex = 0)
        {
            var index = Math.Max(0, _index + relativeIndex);
            if (index >= _length)
            {
                return false;
            }
            return Text.IndexOf(c, index) >= 0;
        }

        public bool Contains(string str, int relativeIndex = 0)
        {
            var index = Math.Max(0, _index + relativeIndex);
            if (index >= _length)
            {
                return false;
            }
            return Text.IndexOf(str, index) >= 0;
        }

        public bool StartsWith(string str, int relativeIndex = 0)
        {
            var index = _index + relativeIndex;
            if (index + str.Length > _length)
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

        public bool PartOfSequence(char c, int count)
        {
            var index = _index;
            while (index > 0 && Text[index] == c)
            {
                index -= 1;
            }
            index += 1;
            while (index < _length && count > 0 && Text[index] == c)
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

        #if DEBUG
        
        public override string ToString()
        {
            return string.Format("{0}↑{1}", Escape(Text.Substring(0, _index)), Escape(Text.Substring(_index)));
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

        #endif

        public class SavedSubject
        {
            public SavedSubject(Subject subject)
            {
                Subject = subject;
                Index = Subject._index;
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

            #if DEBUG

            public override string ToString()
            {
                return string.Format("{0}↑{1}↑{2}", Escape(Subject.Text.Substring(0, Index)), Escape(Subject.Text.Substring(Index, Subject.Index - Index)), Escape(Subject.Text.Substring(Subject.Index)));
            }

            #endif
        }
    }
}
