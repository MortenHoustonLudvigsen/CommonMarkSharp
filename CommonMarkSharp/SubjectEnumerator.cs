using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp
{
    public class SubjectEnumerator : IDisposable
    {
        public SubjectEnumerator(string text)
        {
            _enumerator = text.GetEnumerator();
            EndOfString = !_enumerator.MoveNext();
            Current = EndOfString ? char.MinValue : _enumerator.Current;
        }

        private SubjectEnumerator(SubjectEnumerator subjectEnumerator)
        {
            _enumerator = (CharEnumerator)subjectEnumerator._enumerator.Clone();
            EndOfString = subjectEnumerator.EndOfString;
            Index = subjectEnumerator.Index;
            Current = subjectEnumerator.Current;
        }

        private CharEnumerator _enumerator;
        public bool EndOfString = false;
        public int Index = 0;
        public char Current;

        public bool MoveNext()
        {
            if (!EndOfString)
            {
                Index += 1;
                if (_enumerator.MoveNext())
                {
                    Current = _enumerator.Current;
                    return true;
                }
                Current = char.MinValue;
                EndOfString = true;
            }
            return false;
        }

        public SubjectEnumerator Clone()
        {
            return new SubjectEnumerator(this);
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
    }
}
