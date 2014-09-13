using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonMarkSharp
{
    public static class RegexUtils
    {
        public const RegexOptions Options = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;

        public static string Join(params string[] patterns)
        {
            return "(?:" + string.Join("|", patterns) + ")";
        }

        public static Regex Create(string pattern)
        {
            return new Regex(pattern, Options);
        }

        public static Regex Create(string pattern, params string[] args)
        {
            return new Regex(string.Format(pattern, args), Options);
        }

        public static bool IsMatch(string input, string pattern, out string[] groups)
        {
            return Create(pattern).IsMatch(input, out groups);
        }

        public static bool IsMatch(string input, string pattern, int index, out string[] groups)
        {
            return Create(pattern).IsMatch(input, index, out groups);
        }

        public static bool IsMatch(this Regex re, string input, out string[] groups)
        {
            var match = re.Match(input);
            if (match.Success)
            {
                groups = match.Groups.Cast<Group>().Select(g => g.Value).ToArray();
                return true;
            }
            groups = new string[] { };
            return false;
        }

        public static bool IsMatch(this Regex re, string input, int index, out string[] groups)
        {
            if (index >= 0 && index < input.Length)
            {
                var match = re.Match(input, index);
                if (match.Success)
                {
                    groups = match.Groups.Cast<Group>().Select(g => g.Value).ToArray();
                    return true;
                }
            }
            groups = new string[] { };
            return false;
        }
    }
}
