using CommonMarkSharp;
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
    public class AutolinkParser : IParser<Link>
    {
        // scheme = 'coap'|'doi'|'javascript'|'aaa'|'aaas'|'about'|'acap'|'cap'|'cid'|'crid'|'data'|'dav'|
        //          'dict'|'dns'|'file'|'ftp'|'geo'|'go'|'gopher'|'h323'|'http'|'https'|'iax'|'icap'|'im'|
        //          'imap'|'info'|'ipp'|'iris'|'iris.beep'|'iris.xpc'|'iris.xpcs'|'iris.lwz'|'ldap'|'mailto'|
        //          'mid'|'msrp'|'msrps'|'mtqp'|'mupdate'|'news'|'nfs'|'ni'|'nih'|'nntp'|'opaquelocktoken'|
        //          'pop'|'pres'|'rtsp'|'service'|'session'|'shttp'|'sieve'|'sip'|'sips'|'sms'|'snmp'|
        //          'soap.beep'|'soap.beeps'|'tag'|'tel'|'telnet'|'tftp'|'thismessage'|'tn3270'|'tip'|'tv'|
        //          'urn'|'vemmi'|'ws'|'wss'|'xcon'|'xcon-userid'|'xmlrpc.beep'|'xmlrpc.beeps'|'xmpp'|'z39.50r'|
        //          'z39.50s'|'adiumxtra'|'afp'|'afs'|'aim'|'apt'|'attachment'|'aw'|'beshare'|'bitcoin'|
        //          'bolo'|'callto'|'chrome'|'chrome-extension'|'com-eventbrite-attendee'|'content'|'cvs'|
        //          'dlna-playsingle'|'dlna-playcontainer'|'dtn'|'dvb'|'ed2k'|'facetime'|'feed'|'finger'|'fish'|
        //          'gg'|'git'|'gizmoproject'|'gtalk'|'hcp'|'icon'|'ipn'|'irc'|'irc6'|'ircs'|'itms'|'jar'|'jms'|
        //          'keyparc'|'lastfm'|'ldaps'|'magnet'|'maps'|'market'|'message'|'mms'|'ms-help'|'msnim'|
        //          'mumble'|'mvn'|'notes'|'oid'|'palm'|'paparazzi'|'platform'|'proxy'|'psyc'|'query'|'res'|
        //          'resource'|'rmi'|'rsync'|'rtmp'|'secondlife'|'sftp'|'sgn'|'skype'|'smb'|'soldat'|'spotify'|
        //          'ssh'|'steam'|'svn'|'teamspeak'|'things'|'udp'|'unreal'|'ut2004'|'ventrilo'|'view-source'|
        //          'webcal'|'wtai'|'wyciwyg'|'xfire'|'xri'|'ymsgr';

        private static readonly string[] _schemes = new[]
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
        };

        private const string _nonUriChars = "\x00\x01\x02\x03\x04\x05\x06\x07\x08\x09\x20<>";

        public AutolinkParser(Parsers parsers)
        {
            Parsers = parsers;
            _uriParser = new Lazy<IParser<InlineString>>(() => new CompositeParser<InlineString>(
                parsers.EntityParser,
                new AllExceptParser(_nonUriChars)
            ));
        }

        public Parsers Parsers { get; private set; }
        public Lazy<IParser<InlineString>> _uriParser { get; private set; }

        public string StartsWithChars
        {
            get { return "<"; }
        }

        public Link Parse(ParserContext context, Subject subject)
        {
            if (!this.CanParse(subject)) return null;

            var savedSubject = subject.Save();
            subject.Advance();
            var scheme = subject.TakeWhile(c => c != ':');

            if (subject.Char == ':' && _schemes.Contains(scheme, StringComparer.InvariantCultureIgnoreCase))
            {
                subject.Advance();
                var inlines = new List<InlineString>(new[] { new InlineString(scheme + ":") });
                var uriInlines = _uriParser.Value.ParseMany(context, subject);
                if (subject.Char == '>' && uriInlines.Any())
                {
                    subject.Advance();
                    inlines.AddRange(uriInlines);
                    var uri = string.Join("", inlines.Select(i => i.Value));
                    return new Link(
                        new LinkLabel(uri, inlines),
                        new LinkDestination(uri),
                        new LinkTitle()
                    );
                }
            }

            savedSubject.Restore();
            return null;
        }
    }
}
