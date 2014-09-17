using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonMarkSharp
{
    public abstract class MarkupRenderer : TextRenderer
    {
        public class Attribute
        {
            public enum AttributeValueType
            {
                String,
                Part,
                PartList
            }

            private Attribute(string name, object value, AttributeValueType valueType, bool writeIfEmpty)
            {
                Name = name;
                Value = value;
                ValueType = valueType;
                WriteIfEmpty = writeIfEmpty;
            }

            public Attribute(string name, string value, bool writeIfEmpty = false)
                : this(name, value, AttributeValueType.String, writeIfEmpty)
            {
            }

            public Attribute(string name, Part value, bool writeIfEmpty = false)
                : this(name, value, AttributeValueType.Part, writeIfEmpty)
            {
            }

            public Attribute(string name, IEnumerable<Part> value, bool writeIfEmpty = false)
                : this(name, value, AttributeValueType.PartList, writeIfEmpty)
            {
            }

            public string Name { get; private set; }
            public object Value { get; private set; }
            public AttributeValueType ValueType { get; private set; }
            public bool WriteIfEmpty { get; set; }
        }

        protected bool InAttribute { get; set; }

        protected virtual void WriteAttribute<T>(Part part, string attribute, T value, bool includeEmptyAttribute, Func<T, bool> shouldWriteAttribute, Action<T> writeValue)
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

        protected virtual void WriteAttribute(Part part, Attribute attribute)
        {
            switch (attribute.ValueType)
            {
                case Attribute.AttributeValueType.String:
                    WriteAttribute(part, attribute.Name, attribute.Value as string, attribute.WriteIfEmpty, v => !string.IsNullOrEmpty(v), v => WriteEscaped(v, true));
                    break;
                case Attribute.AttributeValueType.Part:
                    WriteAttribute(part, attribute.Name, attribute.Value as Part, attribute.WriteIfEmpty, p => p != null, p => p.Accept(this));
                    break;
                case Attribute.AttributeValueType.PartList:
                    WriteAttribute(part, attribute.Name, attribute.Value as IEnumerable<Part>, attribute.WriteIfEmpty, p => p != null && p.Any(), p => p.Accept(this));
                    break;
            }
        }

        protected virtual void WriteAttributes(Part part, List<Attribute> attributes)
        {
            foreach (var attribute in attributes)
            {
                WriteAttribute(part, attribute);
            }
        }

        protected void WriteStartTag(Part part, string tag, params Attribute[] attributes)
        {
            WriteStartTag(part, tag, attributes.ToList());
        }

        protected virtual void WriteStartTag(Part part, string tag, List<Attribute> attributes)
        {
            WriteEscapedInAttribute("<");
            WriteEscapedInAttribute(tag);
            WriteAttributes(part, attributes);
            WriteEscapedInAttribute(">");
        }

        protected virtual void WriteEndTag(Part part, string tag)
        {
            WriteEscapedInAttribute("</");
            WriteEscapedInAttribute(tag);
            WriteEscapedInAttribute(">");
        }

        protected void WriteClosedTag(Part part, string tag, params Attribute[] attributes)
        {
            WriteClosedTag(part, tag, attributes.ToList());
        }

        protected virtual void WriteClosedTag(Part part, string tag, List<Attribute> attributes)
        {
            WriteEscapedInAttribute("<");
            WriteEscapedInAttribute(tag);
            WriteAttributes(part, attributes);
            WriteEscapedInAttribute(" />");
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

        protected virtual void WriteEscapedInAttribute(string str)
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
    }
}
