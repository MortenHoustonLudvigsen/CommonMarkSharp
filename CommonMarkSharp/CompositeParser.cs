using CommonMarkSharp.Blocks;
using CommonMarkSharp.InlineParsers;
using CommonMarkSharp.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp
{
    public class CompositeParser<T> : IParser<T>
        where T: class
    {
        public CompositeParser(params IParser<T>[] parsers)
        {
            _parsers = parsers.ToList();
            if (_parsers.Any(p => p.StartsWithChars == null))
            {
                StartsWithChars = null;
            }
            else
            {
                StartsWithChars = new string(_parsers.SelectMany(p => p.StartsWithChars).Distinct().ToArray());
            }
        }

        private readonly IEnumerable<IParser<T>> _parsers;

        public virtual string StartsWithChars { get; private set; }

        public virtual bool CanParse(Subject subject)
        {
            return StartsWithChars == null || StartsWithChars.Contains(subject.Char);
        }

        public virtual T Parse(ParserContext context, Subject subject)
        {
            if (!CanParse(subject)) return null;

            foreach (var parser in _parsers)
            {
                var result = parser.Parse(context, subject);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
