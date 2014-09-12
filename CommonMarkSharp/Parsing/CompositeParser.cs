using CommonMarkSharp.Parsing.Blocks;
using CommonMarkSharp.Parsing.InlineParsers;
using CommonMarkSharp.Parsing.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Parsing
{
    public class CompositeParser<TPart> : IParser<TPart>
        where TPart : Part
    {
        public CompositeParser(params IParser<TPart>[] parsers)
        {
            _startsWithChars = new Lazy<string>(() =>
            {
                InitializeInternal();
                if (_parsers.Any(p => p.StartsWithChars == null))
                {
                    return null;
                }
                return new string(_parsers.SelectMany(p => p.StartsWithChars).Distinct().ToArray());
            });
            Register(parsers);
        }

        private readonly Lazy<string> _startsWithChars;
        private readonly List<IParser<TPart>> _parsers = new List<IParser<TPart>>();

        private bool _initialized = false;
        private void InitializeInternal()
        {
            if (!_initialized)
            {
                _initialized = true;
                Initialize();
            }
        }

        protected virtual void Initialize()
        {
        }

        public void Register(params IParser<TPart>[] parsers)
        {
            Register(parsers.AsEnumerable());
        }

        public void Register(IEnumerable<IParser<TPart>> parsers)
        {
            foreach (var parser in parsers)
            {
                Register(parser);
            }
        }

        public void Register(IParser<TPart> parser)
        {
            InitializeInternal();
            if (parser == null) throw new ArgumentNullException("parser");
            _parsers.Add(parser);
        }

        public virtual string StartsWithChars { get { return _startsWithChars.Value; } }

        public virtual TPart Parse(ParserContext context, Subject subject)
        {
            InitializeInternal();
            if (!this.CanParse(subject)) return null;
            var savedSubject = subject.Save();
            foreach (var parser in _parsers)
            {
                var part = parser.Parse(context, subject);
                if (part != null)
                {
                    return part;
                }
            }

            savedSubject.Restore();
            return null;
        }
    }
}
