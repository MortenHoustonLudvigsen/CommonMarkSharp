using CommonMarkSharp.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp
{
    public abstract class MarkupRenderer : TextRenderer
    {
        protected bool InAttribute { get; set; }

        protected virtual void WriteAttribute<T>(string attribute, T value, bool includeEmptyAttribute, Func<T, bool> shouldWriteAttribute, Action<T> writeValue)
            where T: class
        {
            if (includeEmptyAttribute || shouldWriteAttribute(value))
            {
                Write(" {0}=\"", attribute);
                InAttribute = true;
                if (value != null)
                {
                    writeValue(value);
                }
                InAttribute = false;
                Write("\"");
            }
        }

        protected virtual void WriteAttribute(string attribute, string value, bool includeEmptyAttribute = false)
        {
            WriteAttribute(attribute, value, includeEmptyAttribute, v => !string.IsNullOrEmpty(v), v => Write(v));
        }

        protected virtual void WriteAttribute(string attribute, Part part, bool includeEmptyAttribute = false)
        {
            WriteAttribute(attribute, part, includeEmptyAttribute, p => p != null, p => p.Accept(this));
        }

        protected virtual void WriteAttribute(string attribute, IEnumerable<Part> parts, bool includeEmptyAttribute = false)
        {
            WriteAttribute(attribute, parts, includeEmptyAttribute, p => p != null && p.Any(), p => p.Accept(this));
        }

        protected virtual string Escape(string str, bool preserveEntities = false)
        {
            if (!preserveEntities)
            {
                str = str.Replace("&", "&amp;");
            }
            str = str.Replace("<", "&lt;");
            str = str.Replace(">", "&gt;");
            str = str.Replace("\"", "&quot;");
            return str;
        }

        protected virtual string EscapeInAttribute(string str)
        {
            return InAttribute ? Escape(str) : str;
        }
    }
}
