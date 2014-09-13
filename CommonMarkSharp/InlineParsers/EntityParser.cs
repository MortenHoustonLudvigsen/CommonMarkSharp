using CommonMarkSharp.Blocks;
using CommonMarkSharp.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonMarkSharp.InlineParsers
{
    public class EntityParser : RegexParser<InlineEntity>
    {
        public const string NamedEntity = @"[A-Za-z][A-Za-z0-9]{1,31}";
        public const string DecimalEntity = @"#[0-9]{1,8}";
        public const string HexadecimalEntity = @"#[Xx][0-9A-Fa-f]{1,8}";
        public static readonly string Entity = string.Format("&{0};", RegexUtils.Join(NamedEntity, DecimalEntity, HexadecimalEntity));
        public static readonly Regex EntityRe = RegexUtils.Create(@"\G{0}", Entity);

        public EntityParser()
            : base(EntityRe)
        {
            StartsWithChars = "&";
        }

        protected override InlineEntity HandleMatch(ParserContext context, string[] groups)
        {
            return new InlineEntity(groups[0]);
        }
    }
}
