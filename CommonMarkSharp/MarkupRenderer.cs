using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonMarkSharp
{
    public abstract class MarkupRenderer : TextRenderer
    {
        protected bool InAttribute { get; set; }

        protected virtual void WriteAttribute<T>(string attribute, T value, bool includeEmptyAttribute, Func<T, bool> shouldWriteAttribute, Action<T> writeValue)
            where T : class
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
            WriteAttribute(attribute, value, includeEmptyAttribute, v => !string.IsNullOrEmpty(v), v => WriteEscaped(v, true));
        }

        protected virtual void WriteAttribute(string attribute, Part part, bool includeEmptyAttribute = false)
        {
            WriteAttribute(attribute, part, includeEmptyAttribute, p => p != null, p => p.Accept(this));
        }

        protected virtual void WriteAttribute(string attribute, IEnumerable<Part> parts, bool includeEmptyAttribute = false)
        {
            WriteAttribute(attribute, parts, includeEmptyAttribute, p => p != null && p.Any(), p => p.Accept(this));
        }

        protected virtual void WriteEscaped(string str, bool preserveEntities = false)
        {
            var chars = preserveEntities ? new[] { '<', '>', '"' } : new[] { '&', '<', '>', '"' };

            var start = 0;
            var pos = str.IndexOfAny(chars);
            while (pos >= 0)
            {
                if (pos > start)
                {
                    Write(str.Substring(start, pos - start));
                }
                switch (str[pos])
                {
                    case '&':
                        Write("&amp;");
                        break;
                    case '<':
                        Write("&lt;");
                        break;
                    case '>':
                        Write("&gt;");
                        break;
                    case '"':
                        Write("&quot;");
                        break;
                }
                start = pos + 1;
                pos = str.IndexOfAny(chars, start);
            }

            if (start == 0)
            {
                Write(str);
            }
            else
            {
                Write(str.Substring(start));
            }
        }

        protected virtual void WriteEscapedInAttribute(string str, bool preserveEntities = false)
        {
            if (InAttribute)
            {
                WriteEscaped(str);
            }
            else
            {
                Write(str);
            }
        }

        //protected virtual string Escape(string str, bool preserveEntities = false)
        //{
        //    if (!preserveEntities)
        //    {
        //        str = str.Replace("&", "&amp;");
        //    }
        //    str = str.Replace("<", "&lt;");
        //    str = str.Replace(">", "&gt;");
        //    str = str.Replace("\"", "&quot;");
        //    return str;
        //}

        //protected virtual string EscapeInAttribute(string str)
        //{
        //    return InAttribute ? Escape(str) : str;
        //}
    }
}
