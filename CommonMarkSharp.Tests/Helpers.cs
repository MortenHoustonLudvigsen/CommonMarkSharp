using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonMarkSharp.Tests
{
    public static class Helpers
    {
        public static string Normalize(string value)
        {
            value = value.Replace('→', '\t');
            value = value.Replace('␣', ' ');
            return value;
        }

        public static string Tidy(string html)
        {
            var result = new List<string>();
            var inPre = false;
            foreach (var line in html.Split('\n'))
            {
                if (Regex.IsMatch(line, @"<pre"))
                {
                    inPre = true;
                }
                else if (Regex.IsMatch(line, @"</pre"))
                {
                    inPre = false;
                }
                if (inPre)
                {
                    result.Add(line);
                }
                else
                {
                    var tidyLine = line;
                    // remove leading spaces
                    tidyLine = Regex.Replace(line, @"^ +", "");
                    // remove trailing spaces
                    tidyLine = Regex.Replace(line, @" +$", "");
                    // collapse consecutive spaces
                    tidyLine = Regex.Replace(line, @" +", " ");
                    // collapse space before /> in tag
                    tidyLine = Regex.Replace(line, @" +/>", "/>");
                    // skip blank line
                    if (tidyLine != "")
                    {
                        result.Add(tidyLine);
                    }
                }
            }
            return string.Join("\n", result);
        }

        public static void Log()
        {
            Console.WriteLine();
        }

        public static void Log(string message)
        {
            Console.WriteLine(message);
        }

        public static void Log(string format, params object[] args)
        {
            Log(string.Format(format, args));
        }

        public static void LogValue(string caption, string text)
        {
            Log("{0}:", caption);
            Log();
            Log(string.Join("\n", text.Replace("\r", "").Split('\n', '\r').Select(s => "    " + s)));
            Log();
        }
    }
}
