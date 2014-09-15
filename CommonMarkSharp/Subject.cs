using System;
using System.Runtime.CompilerServices;

namespace CommonMarkSharp
{
    public class Subject
    {
        private static readonly string[] _emptyGroups = new string[0];

        public Subject(string text)
        {
            Text = text ?? "";
            _length = Text.Length;
            _firstNonSpace = null;
            Index = 0;

            if (_length == 0)
            {
                EndOfString = true;
                Char = char.MinValue;
            }
            else
            {
                EndOfString = false;
                Char = Text[0];
            }
        }

        private int _length;
        public int Index;
        public char Char;
        public bool EndOfString;
        public string Text;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetIndex(int value)
        {
            if (value != Index)
            {
                if (value >= _length)
                {
                    Index = _length;
                    EndOfString = true;
                    Char = char.MinValue;
                }
                else
                {
                    Index = value;
                    //EndOfString = false;
                    Char = Text[value];
                }
                _firstNonSpace = null;
            }
        }


        private int? _firstNonSpace;
        public int FirstNonSpace
        {
            get
            {
                if (_firstNonSpace == null)
                {
                    _firstNonSpace = Index + this.CountWhile(' ');
                }
                return _firstNonSpace.Value;
            }
        }

        public char FirstNonSpaceChar { get { return GetChar(FirstNonSpace); } }
        public bool IsBlank { get { return FirstNonSpace == _length; } }
        public int Indent { get { return FirstNonSpace - Index; } }
        public string Rest { get { return EndOfString ? "" : Text.Substring(Index); } }

        public char this[int relativeIndex]
        {
            get { return GetChar(Index + relativeIndex); }
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

        public bool Advance()
        {
            if (EndOfString)
            {
                return false;
            }
            SetIndex(Index + 1);
            return true;
        }

        public int Advance(int count)
        {
            if (count != 0)
            {
                var originalIndex = Index;
                SetIndex(Index + count);
                return Index - originalIndex;
            }
            return 0;
        }


        public void AdvanceToFirstNonSpace(int offset = 0)
        {
            SetIndex(Math.Min(FirstNonSpace + offset, _length));
        }

        public void AdvanceToEnd(int offset = 0)
        {
            SetIndex(_length + offset);
        }

        public bool Contains(char c, int relativeIndex = 0)
        {
            var index = Math.Max(0, Index + relativeIndex);
            if (index >= _length)
            {
                return false;
            }
            return Text.IndexOf(c, index) >= 0;
        }

        public bool Contains(string str, int relativeIndex = 0)
        {
            var index = Math.Max(0, Index + relativeIndex);
            if (index >= _length)
            {
                return false;
            }
            return Text.IndexOf(str, index) >= 0;
        }

        public bool StartsWith(string str, int relativeIndex = 0)
        {
            var index = Index + relativeIndex;
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
            var index = Index;
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
            return this.AdvanceWhile(c => IsWhiteSpace(c));
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

#endif

        public struct SavedSubject
        {
            public SavedSubject(Subject subject)
            {
                _subject = subject;
                _index = _subject.Index;
                _char = subject.Char;
                _endOfString = subject.EndOfString;
            }

            private Subject _subject;
            private int _index;
            private char _char;
            private bool _endOfString;

            public void Restore()
            {
                if (_index != _subject.Index)
                {
                    _subject.Index = _index;
                    _subject.EndOfString = false;
                    _subject.Char = _char;
                    _subject._firstNonSpace = null;
                }
            }

            public string GetLiteral()
            {
                return _subject.Text.Substring(_index, _subject.Index - _index);
            }

#if DEBUG

            public override string ToString()
            {
                return string.Format("{0}↑{1}↑{2}", Escape(_subject.Text.Substring(0, _index)), Escape(_subject.Text.Substring(_index, _subject.Index - _index)), Escape(_subject.Text.Substring(_subject.Index)));
            }

#endif
        }
    }
}
