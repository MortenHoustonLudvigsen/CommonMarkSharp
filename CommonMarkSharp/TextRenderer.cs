using CommonMarkSharp.Blocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp
{
    public abstract class TextRenderer : CommonMarkVisitor
    {
        private TextWriter _writer;

        public void Render(Document document, TextWriter writer)
        {
            _writer = writer;
            document.Accept(this);
        }

        public string Render(Document document)
        {
            using (var writer = new StringWriter())
            {
                Render(document, writer);
                return writer.ToString();
            }
        }

        private bool _shouldWriteLine = false;

        protected virtual void Write(string str)
        {
            if (_shouldWriteLine)
            {
                _writer.Write("\n");
                _shouldWriteLine = false;
            }
            _writer.Write(str);
        }

        protected virtual void Write(string format, params object[] args)
        {
            Write(string.Format(format, args));
        }

        protected virtual void WriteLine()
        {
            _shouldWriteLine = true;
        }

        protected virtual void WriteLine(string str)
        {
            Write(str);
            WriteLine();
        }

        protected virtual void WriteLine(string str, params object[] args)
        {
            Write(str, args);
            WriteLine();
        }

        protected virtual void Write(IEnumerable<Part> parts)
        {
            if (parts != null)
            {
                parts.Accept(this);
            }
        }
    }
}
