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
    public class AutolinkParser : RegexParser<Link>
    {
        // scheme = 'coap'|'doi'|'javascript'|'aaa'|'aaas'|'about'|'acap'|'cap'|'cid'|'crid'|'data'|'dav'|'dict'|'dns'|'file'|'ftp'|'geo'|'go'|'gopher'|'h323'|'http'|'https'|'iax'|'icap'|'im'|'imap'|'info'|'ipp'|'iris'|'iris.beep'|'iris.xpc'|'iris.xpcs'|'iris.lwz'|'ldap'|'mailto'|'mid'|'msrp'|'msrps'|'mtqp'|'mupdate'|'news'|'nfs'|'ni'|'nih'|'nntp'|'opaquelocktoken'|'pop'|'pres'|'rtsp'|'service'|'session'|'shttp'|'sieve'|'sip'|'sips'|'sms'|'snmp'|'soap.beep'|'soap.beeps'|'tag'|'tel'|'telnet'|'tftp'|'thismessage'|'tn3270'|'tip'|'tv'|'urn'|'vemmi'|'ws'|'wss'|'xcon'|'xcon-userid'|'xmlrpc.beep'|'xmlrpc.beeps'|'xmpp'|'z39.50r'|'z39.50s'|'adiumxtra'|'afp'|'afs'|'aim'|'apt'|'attachment'|'aw'|'beshare'|'bitcoin'|'bolo'|'callto'|'chrome'|'chrome-extension'|'com-eventbrite-attendee'|'content'|'cvs'|'dlna-playsingle'|'dlna-playcontainer'|'dtn'|'dvb'|'ed2k'|'facetime'|'feed'|'finger'|'fish'|'gg'|'git'|'gizmoproject'|'gtalk'|'hcp'|'icon'|'ipn'|'irc'|'irc6'|'ircs'|'itms'|'jar'|'jms'|'keyparc'|'lastfm'|'ldaps'|'magnet'|'maps'|'market'|'message'|'mms'|'ms-help'|'msnim'|'mumble'|'mvn'|'notes'|'oid'|'palm'|'paparazzi'|'platform'|'proxy'|'psyc'|'query'|'res'|'resource'|'rmi'|'rsync'|'rtmp'|'secondlife'|'sftp'|'sgn'|'skype'|'smb'|'soldat'|'spotify'|'ssh'|'steam'|'svn'|'teamspeak'|'things'|'udp'|'unreal'|'ut2004'|'ventrilo'|'view-source'|'webcal'|'wtai'|'wyciwyg'|'xfire'|'xri'|'ymsgr';
        public static readonly string Scheme = RegexUtils.Join(new[]
        {
            "coap", "doi", "javascript", "aaa", "aaas", "about", "acap", "cap", "cid",
            "crid", "data", "dav", "dict", "dns", "file", "ftp", "geo", "go", "gopher",
            "h323", "http", "https", "iax", "icap", "im", "imap", "info", "ipp", "iris",
            "iris.beep", "iris.xpc", "iris.xpcs", "iris.lwz", "ldap", "mailto", "mid",
            "msrp", "msrps", "mtqp", "mupdate", "news", "nfs", "ni", "nih", "nntp",
            "opaquelocktoken", "pop", "pres", "rtsp", "service", "session", "shttp",
            "sieve", "sip", "sips", "sms", "snmp", "soap.beep", "soap.beeps", "tag",
            "tel", "telnet", "tftp", "thismessage", "tn3270", "tip", "tv", "urn", "vemmi",
            "ws", "wss", "xcon", "xcon-userid", "xmlrpc.beep", "xmlrpc.beeps", "xmpp",
            "z39.50r", "z39.50s", "adiumxtra", "afp", "afs", "aim", "apt", "attachment",
            "aw", "beshare", "bitcoin", "bolo", "callto", "chrome", "chrome-extension",
            "com-eventbrite-attendee", "content", "cvs", "dlna-playsingle",
            "dlna-playcontainer", "dtn", "dvb", "ed2k", "facetime", "feed", "finger",
            "fish", "gg", "git", "gizmoproject", "gtalk", "hcp", "icon", "ipn", "irc",
            "irc6", "ircs", "itms", "jar", "jms", "keyparc", "lastfm", "ldaps", "magnet",
            "maps", "market", "message", "mms", "ms-help", "msnim", "mumble", "mvn", "notes",
            "oid", "palm", "paparazzi", "platform", "proxy", "psyc", "query", "res", "resource",
            "rmi", "rsync", "rtmp", "secondlife", "sftp", "sgn", "skype", "smb", "soldat",
            "spotify", "ssh", "steam", "svn", "teamspeak", "things", "udp", "unreal", "ut2004",
            "ventrilo", "view-source", "webcal", "wtai", "wyciwyg", "xfire", "xri", "ymsgr"
        });

        // Try to match URI autolink after first <.
        //     scheme [:]([^\x00-\x20<>\\]|escaped_char)*[>]
        public static readonly string AutolinkUri = string.Format(@"{0}:{1}*", Scheme, RegexUtils.Join(@"[^\x00-\x20<>\\]", Patterns.EscapedChar));
        public static readonly Regex AutolinkUriRe = RegexUtils.Create(@"\G<({0})>", AutolinkUri);

        public AutolinkParser(Parsers parsers)
            : base(AutolinkUriRe)
        {
            Parsers = parsers;
            StartsWithChars = "<";
        }

        public Parsers Parsers { get; private set; }

        protected override Link HandleMatch(ParserContext context, string[] groups)
        {
            return new Link(
                new LinkLabel(groups[1], Parsers.StrWithEntitiesParser.ParseMany(context, new Subject(groups[1]))),
                new LinkDestination(groups[1]),
                new LinkTitle()
            );
        }
    }
}
