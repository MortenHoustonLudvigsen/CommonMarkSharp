using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace CommonMarkSharp
{
    public class Subject : IDisposable
    {
        private static readonly string[] _emptyGroups = new string[0];

        public Subject(string text)
        {
            Text = text ?? "";
            _length = Text.Length;
            _firstNonSpace = null;
            _enumerator = new SubjectEnumerator(Text);
        }

        public string Text;
        private SubjectEnumerator _enumerator;

        private int _length;

        public char Char { get { return _enumerator.Current; } }
        public bool EndOfString { get { return _enumerator.EndOfString; } }

        private int? _firstNonSpace;
        public int FirstNonSpace
        {
            get
            {
                if (_firstNonSpace == null)
                {
                    _firstNonSpace = _enumerator.Index + CountWhile(' ');
                }
                return _firstNonSpace.Value;
            }
        }

        public char FirstNonSpaceChar { get { return GetChar(FirstNonSpace); } }
        public bool IsBlank { get { return FirstNonSpace == _length; } }
        public int Indent { get { return FirstNonSpace - _enumerator.Index; } }
        public string Rest { get { return EndOfString ? "" : Text.Substring(_enumerator.Index); } }

        public char this[int relativeIndex]
        {
            get { return GetChar(_enumerator.Index + relativeIndex); }
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
            _firstNonSpace = null;
            return _enumerator.MoveNext();
        }

        public int Advance(int count)
        {
            if (EndOfString)
            {
                return 0;
            }
            _firstNonSpace = null;
            var advanced = 0;
            while (advanced < count && _enumerator.MoveNext())
            {
                advanced += 1;
            }
            return advanced;
        }

        public int AdvanceWhile(char c, int max = int.MaxValue)
        {
            return AdvanceWhile(x => c == x, max);
        }

        public int AdvanceWhileNot(char c, int max = int.MaxValue)
        {
            return AdvanceWhile(x => c != x);
        }

        public int AdvanceWhile(Func<char, bool> predicate, int max = int.MaxValue)
        {
            if (EndOfString)
            {
                return 0;
            }
            var advanced = 0;
            var valid = true;
            _firstNonSpace = null;
            while (valid && predicate(_enumerator.Current) && advanced < max)
            {
                advanced += 1;
                valid = _enumerator.MoveNext();
            }
            return advanced;
        }

        public void AdvanceToFirstNonSpace(int offset = 0)
        {
            AdvanceWhile(' ');
            Advance(offset);
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
            var result = Text.Substring(_enumerator.Index, count);
            Advance(count);
            return result;
        }

        public string TakeWhile(char c, int max = int.MaxValue)
        {
            return TakeWhile(x => c == x, max);
        }

        public string TakeWhileNot(char c, int max = int.MaxValue)
        {
            return TakeWhile(x => c != x, max);
        }

        public string TakeWhile(Func<char, bool> predicate, int max = int.MaxValue)
        {
            if (EndOfString)
            {
                return "";
            }
            StringBuilder result = null;
            var advanced = 0;
            var valid = true;
            _firstNonSpace = null;
            while (valid && predicate(_enumerator.Current) && advanced < max)
            {
                result = result ?? new StringBuilder(_length);
                result.Append(_enumerator.Current);
                advanced += 1;
                valid = _enumerator.MoveNext();
            }
            return result != null ? result.ToString() : "";
        }

        public int CountWhile(char c, int max = int.MaxValue)
        {
            return CountWhile(x => c == x, max);
        }

        public int CountWhileNot(char c, int max = int.MaxValue)
        {
            return CountWhile(x => c != x, max);
        }

        public int CountWhile(Func<char, bool> predicate, int max = int.MaxValue)
        {
            if (EndOfString)
            {
                return 0;
            }
            var count = 0;
            using (var enumerator = _enumerator.Clone())
            {
                var valid = true;
                while (valid && predicate(enumerator.Current) && count < max)
                {
                    count += 1;
                    valid = enumerator.MoveNext();
                }
                return count;
            }
        }

        public bool Contains(char c, int relativeIndex = 0)
        {
            var index = Math.Max(0, _enumerator.Index + relativeIndex);
            if (index >= _length)
            {
                return false;
            }
            return Text.IndexOf(c, index) >= 0;
        }

        public bool Contains(string str, int relativeIndex = 0)
        {
            var index = Math.Max(0, _enumerator.Index + relativeIndex);
            if (index >= _length)
            {
                return false;
            }
            return Text.IndexOf(str, index) >= 0;
        }

        public bool StartsWith(string str, int relativeIndex = 0)
        {
            var index = _enumerator.Index + relativeIndex;
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
            var index = _enumerator.Index;
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

        private bool _disposed = false;
        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern. 
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                if (_enumerator != null)
                {
                    _enumerator.Dispose();
                    _enumerator = null;
                }
            }
            _disposed = true;
        }

#if DEBUG

        public override string ToString()
        {
            return string.Format("{0}↑{1}", Escape(Text.Substring(0, _enumerator.Index)), Escape(Text.Substring(_enumerator.Index)));
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
                _enumerator = subject._enumerator.Clone();
            }

            private Subject _subject;
            private SubjectEnumerator _enumerator;

            public void Restore()
            {
                _subject._enumerator.Dispose();
                _subject._enumerator = _enumerator;
            }

            public string GetLiteral()
            {
                return _subject.Text.Substring(_enumerator.Index, _subject._enumerator.Index - _enumerator.Index);
            }

#if DEBUG

            public override string ToString()
            {
                return string.Format("{0}↑{1}↑{2}", Escape(_subject.Text.Substring(0, _enumerator.Index)), Escape(_subject.Text.Substring(_enumerator.Index, _subject._enumerator.Index - _enumerator.Index)), Escape(_subject.Text.Substring(_subject._enumerator.Index)));
            }

#endif
        }
    }
}
