using System.Text.RegularExpressions;

namespace CommonMarkSharp
{
    public static class Patterns
    {
        public const string ControlChars = "\x00\x01\x02\x03\x04\x05\x06\x07\x08\x09\x0A\x0B\x0C\x0D\x0E\x0F"
                                         + "\x10\x11\x12\x13\x14\x15\x16\x17\x18\x19\x1A\x1B\x1C\x1D\x1E\x1F\x20";

        public const string UpperCaseAlphas = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string LowerCaseAlphas = "abcdefghijklmnopqrstuvwxyz";
        public const string Alphas = UpperCaseAlphas + LowerCaseAlphas;
        public const string Digits = "0123456789";
        public const string Alphanums = Alphas + Digits;
        public const string HexDigits = "0123456789ABCDEFabcdef";

        // spacechar = [ \t\n];
        public static readonly string SpaceChar = @"[ \t\n]";

        //// reg_char     = [^\\()\x00-\x20];
        //public static readonly string RegChar = @"[^\\()\x00-\x20]";

        // escaped_char = [\\][!"#$%&'()*+,./:;<=>?@[\\\]^_`{|}~-];
        public static readonly string EscapedChar = @"\\[!""#$%&'()*+,./:;<=>?@[\\\]^_`{|}~-]";

        //public static string PunctuationChars = @"!""#$%&'()*+,-./:;<=>?@[\]^_`{|}~";
        //public static string SpecialChars = "\n\\`&_*[]<!";

        // blocktagname = 'article'|'header'|'aside'|'hgroup'|'iframe'|'blockquote'|'hr'|'body'|'li'|'map'|'button'|'object'|'canvas'|'ol'|'caption'|'output'|'col'|'p'|'colgroup'|'pre'|'dd'|'progress'|'div'|'section'|'dl'|'table'|'td'|'dt'|'tbody'|'embed'|'textarea'|'fieldset'|'tfoot'|'figcaption'|'th'|'figure'|'thead'|'footer'|'footer'|'tr'|'form'|'ul'|'h1'|'h2'|'h3'|'h4'|'h5'|'h6'|'video'|'script'|'style';
        public static readonly string BlockTagName = RegexUtils.Join(new[]
        {
            "article", "header", "aside", "hgroup", "iframe", "blockquote", "hr", "body",
            "li", "map", "button", "object", "canvas", "ol", "caption", "output", "col",
            "p", "colgroup", "pre", "dd", "progress", "div", "section", "dl", "table",
            "td", "dt", "tbody", "embed", "textarea", "fieldset", "tfoot", "figcaption",
            "th", "figure", "thead", "footer", "footer", "tr", "form", "ul", "h1", "h2",
            "h3", "h4", "h5", "h6", "video", "script", "style"
        });

        //// in_parens_nosp   = [(] (reg_char|escaped_char)* [)];
        //public static readonly string InParensNosp = Format(@"\({0}*\)", Join(RegChar, EscapedChar));

        //// in_double_quotes = ["] (escaped_char|[^"\x00])* ["];
        //public static readonly string InDoubleQuotes = Format(@"""{0}*""", Join(EscapedChar, @"[^""\x00]"));
        //// in_single_quotes = ['] (escaped_char|[^'\x00])* ['];
        //public static readonly string InSingleQuotes = Format(@"'{0}*'", Join(EscapedChar, @"[^'\x00]"));
        //// in_parens        = [(] (escaped_char|[^)\x00])* [)];
        //public static readonly string InParens = Format(@"\({0}*\)", Join(EscapedChar, @"[^\)\x00]"));

        //// scheme = 'coap'|'doi'|'javascript'|'aaa'|'aaas'|'about'|'acap'|'cap'|'cid'|'crid'|'data'|'dav'|'dict'|'dns'|'file'|'ftp'|'geo'|'go'|'gopher'|'h323'|'http'|'https'|'iax'|'icap'|'im'|'imap'|'info'|'ipp'|'iris'|'iris.beep'|'iris.xpc'|'iris.xpcs'|'iris.lwz'|'ldap'|'mailto'|'mid'|'msrp'|'msrps'|'mtqp'|'mupdate'|'news'|'nfs'|'ni'|'nih'|'nntp'|'opaquelocktoken'|'pop'|'pres'|'rtsp'|'service'|'session'|'shttp'|'sieve'|'sip'|'sips'|'sms'|'snmp'|'soap.beep'|'soap.beeps'|'tag'|'tel'|'telnet'|'tftp'|'thismessage'|'tn3270'|'tip'|'tv'|'urn'|'vemmi'|'ws'|'wss'|'xcon'|'xcon-userid'|'xmlrpc.beep'|'xmlrpc.beeps'|'xmpp'|'z39.50r'|'z39.50s'|'adiumxtra'|'afp'|'afs'|'aim'|'apt'|'attachment'|'aw'|'beshare'|'bitcoin'|'bolo'|'callto'|'chrome'|'chrome-extension'|'com-eventbrite-attendee'|'content'|'cvs'|'dlna-playsingle'|'dlna-playcontainer'|'dtn'|'dvb'|'ed2k'|'facetime'|'feed'|'finger'|'fish'|'gg'|'git'|'gizmoproject'|'gtalk'|'hcp'|'icon'|'ipn'|'irc'|'irc6'|'ircs'|'itms'|'jar'|'jms'|'keyparc'|'lastfm'|'ldaps'|'magnet'|'maps'|'market'|'message'|'mms'|'ms-help'|'msnim'|'mumble'|'mvn'|'notes'|'oid'|'palm'|'paparazzi'|'platform'|'proxy'|'psyc'|'query'|'res'|'resource'|'rmi'|'rsync'|'rtmp'|'secondlife'|'sftp'|'sgn'|'skype'|'smb'|'soldat'|'spotify'|'ssh'|'steam'|'svn'|'teamspeak'|'things'|'udp'|'unreal'|'ut2004'|'ventrilo'|'view-source'|'webcal'|'wtai'|'wyciwyg'|'xfire'|'xri'|'ymsgr';
        //public static readonly string Scheme = Join(new[]
        //{
        //    "coap", "doi", "javascript", "aaa", "aaas", "about", "acap", "cap", "cid",
        //    "crid", "data", "dav", "dict", "dns", "file", "ftp", "geo", "go", "gopher",
        //    "h323", "http", "https", "iax", "icap", "im", "imap", "info", "ipp", "iris",
        //    "iris.beep", "iris.xpc", "iris.xpcs", "iris.lwz", "ldap", "mailto", "mid",
        //    "msrp", "msrps", "mtqp", "mupdate", "news", "nfs", "ni", "nih", "nntp",
        //    "opaquelocktoken", "pop", "pres", "rtsp", "service", "session", "shttp",
        //    "sieve", "sip", "sips", "sms", "snmp", "soap.beep", "soap.beeps", "tag",
        //    "tel", "telnet", "tftp", "thismessage", "tn3270", "tip", "tv", "urn", "vemmi",
        //    "ws", "wss", "xcon", "xcon-userid", "xmlrpc.beep", "xmlrpc.beeps", "xmpp",
        //    "z39.50r", "z39.50s", "adiumxtra", "afp", "afs", "aim", "apt", "attachment",
        //    "aw", "beshare", "bitcoin", "bolo", "callto", "chrome", "chrome-extension",
        //    "com-eventbrite-attendee", "content", "cvs", "dlna-playsingle",
        //    "dlna-playcontainer", "dtn", "dvb", "ed2k", "facetime", "feed", "finger",
        //    "fish", "gg", "git", "gizmoproject", "gtalk", "hcp", "icon", "ipn", "irc",
        //    "irc6", "ircs", "itms", "jar", "jms", "keyparc", "lastfm", "ldaps", "magnet",
        //    "maps", "market", "message", "mms", "ms-help", "msnim", "mumble", "mvn", "notes",
        //    "oid", "palm", "paparazzi", "platform", "proxy", "psyc", "query", "res", "resource",
        //    "rmi", "rsync", "rtmp", "secondlife", "sftp", "sgn", "skype", "smb", "soldat",
        //    "spotify", "ssh", "steam", "svn", "teamspeak", "things", "udp", "unreal", "ut2004",
        //    "ventrilo", "view-source", "webcal", "wtai", "wyciwyg", "xfire", "xri", "ymsgr"
        //});


        //private static readonly RegexOptions ReOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;

        // Try to match an HTML block tag including first <.
        //     [<] [/] blocktagname (spacechar | [>])
        //     [<] blocktagname (spacechar | [/>])
        //     [<] [!?]
        public static readonly string HtmlBlockTagClose = string.Format(@"</{0}(?:{1}|>)", BlockTagName, SpaceChar);
        public static readonly string HtmlBlockTagOpen = string.Format(@"<{0}(?:{1}|/>)", BlockTagName, SpaceChar);
        public static readonly string HtmlBlockTag1 = @"<[!?]";
        public static readonly string HtmlBlockTag = RegexUtils.Join(HtmlBlockTagClose, HtmlBlockTagOpen, HtmlBlockTag1);
        public static readonly Regex HtmlBlockTagRe = new Regex(string.Format(@"\G{0}", HtmlBlockTag), RegexUtils.IgnoreCase);

        //// Try to match a URL in a link or reference.
        //// This may optionally be contained in <..>; otherwise
        //// whitespace and unbalanced right parentheses aren't allowed.
        //// Newlines aren't ever allowed.
        ////     [ \n]* [<] ([^<>\n\\\x00] | escaped_char | [\\])* [>]
        ////     [ \n]* (reg_char+ | escaped_char | in_parens_nosp)*
        //public static readonly string LinkUrl1 = Format(@"[ \n]*<(?:[^<>\n\\\x00]|{0}|[\\])*>", EscapedChar);
        //public static readonly string LinkUrl2 = Format(@"[ \n]*(?:{0}+|{1}|{2})*", RegChar, EscapedChar, InParensNosp);
        //public static readonly string LinkUrl = Join(LinkUrl1, LinkUrl2);
        //public static readonly Regex LinkUrlRe = new Regex(Format(@"\G{0}", LinkUrl), ReOptions);

        //// Try to match a link title (in single quotes, in double quotes, or
        //// in parentheses).  Allow one level of internal nesting (quotes within quotes).
        ////     ["] (escaped_char|[^"\x00])* ["]
        ////     ['] (escaped_char|[^'\x00])* [']
        ////     [(] (escaped_char|[^)\x00])* [)]
        //public static readonly string LinkTitle1 = Format(@"""(?:escaped_char|[^""\x00])*""", EscapedChar);
        //public static readonly string LinkTitle2 = Format(@"'(?:escaped_char|[^'\x00])*'", EscapedChar);
        //public static readonly string LinkTitle3 = Format(@"((?:escaped_char|[^)\x00])*)", EscapedChar);
        //public static readonly string LinkTitle = Join(LinkTitle1, LinkTitle2, LinkTitle3);
        //public static readonly Regex LinkTitleRe = new Regex(Format(@"\G({0})", LinkTitle), ReOptions);

        //// Match space characters, including newlines.
        //public static readonly Regex SpaceCharsRe = new Regex(Format(@"\G(?:{0}*)", SpaceChar), ReOptions);

        // Match ATX header start.
        //     [#]{1,6} ([ ]+|[\n])
        public static readonly Regex ATXHeaderRe = RegexUtils.Create(@"\G(#{1,6})(?: +|$)");
        public static readonly Regex ATXHeaderRemoveTrailingHashRe = RegexUtils.Create(@"(?:(\\#) *#*| *#+) *$");

        //// Match setext header line.
        ////     [=]+ [ ]* [\n]
        ////     [-]+ [ ]* [\n]
        //public static readonly string SetExtHeader1 = @"=+ *\n";
        //public static readonly string SetExtHeader2 = @"-+ *\n";
        //public static readonly string SetExtHeader = Join(SetExtHeader1, SetExtHeader2);
        //public static readonly Regex SetExtHeaderRe = new Regex(Format(@"\G{0}", SetExtHeader), ReOptions);
        public static readonly Regex SetExtHeaderRe = RegexUtils.Create(@"\G(?:=+|-+) *$");

        // Scan a horizontal rule line: "...three or more hyphens, asterisks,
        // or underscores on a line by themselves. If you wish, you may use
        // spaces between the hyphens or asterisks."
        //     ([*][ ]*){3,} [ \t]* [\n]
        //     ([_][ ]*){3,} [ \t]* [\n]
        //     ([-][ ]*){3,} [ \t]* [\n]
        public static readonly string HRule1 = @"(?:\* *){3,}[ \t]*$";
        public static readonly string HRule2 = @"(?:_ *){3,}[ \t]*$";
        public static readonly string HRule3 = @"(?:- *){3,}[ \t]*$";
        public static readonly string HRule = RegexUtils.Join(HRule1, HRule2, HRule3);
        public static readonly Regex HRuleRe = RegexUtils.Create(@"\G{0}", HRule);

        //// Scan an opening code fence.
        ////     [`]{3,} / [^`\n\x00]*[\n]
        ////     [~]{3,} / [^~\n\x00]*[\n]
        //public static readonly string OpenCodeFence1 = @"`{3,}/[^`\n\x00]*\n";
        //public static readonly string OpenCodeFence2 = @"~{3,}/[^~\n\x00]*\n";
        //public static readonly string OpenCodeFence = Join(OpenCodeFence1, OpenCodeFence2);
        //public static readonly Regex OpenCodeFenceRe = new Regex(Format(@"\G{0}", OpenCodeFence), ReOptions);
        public static readonly Regex OpenCodeFenceRe = RegexUtils.Create(@"\G(?:`{3,}(?!.*`)|~{3,}(?!.*~))");
        public static readonly Regex CloseCodeFenceRe = RegexUtils.Create(@"\G(?:`{3,}|~{3,})(?= *$)");
    }
}
