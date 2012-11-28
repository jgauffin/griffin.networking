using System;

namespace Griffin.Networking.Protocol.Http.Implementation
{
    /// <summary>
    /// The Request line has been parsed (first line in a HTTP request)
    /// </summary>
    /// <see cref="HttpHeaderParser.RequestLineParsed"/>
    public class RequestLineEventArgs : EventArgs
    {
        public RequestLineEventArgs(string verb, string url, string httpVersion)
        {
            Verb = verb;
            Url = url;
            HttpVersion = httpVersion;
        }

        /// <summary>
        /// Gets HTTP verb such as "POST"
        /// </summary>
        public string Verb { get; private set; }

        /// <summary>
        /// Gets requested URL (without domain etc)
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Gets http version (for instance "HTTP/1.1")
        /// </summary>
        public string HttpVersion { get; private set; }
    }
}