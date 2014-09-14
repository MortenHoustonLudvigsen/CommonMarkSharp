using System.Collections.Generic;
using System.Linq;

namespace CommonMarkSharp.InlineParsers
{
    public class CompositeInlineParser<T> : IInlineParser<T>
        where T: class
    {
        public CompositeInlineParser(params IInlineParser<T>[] parsers)
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

        private readonly IEnumerable<IInlineParser<T>> _parsers;

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
