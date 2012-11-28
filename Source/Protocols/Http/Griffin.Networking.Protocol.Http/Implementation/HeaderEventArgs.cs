using System;

namespace Griffin.Networking.Protocol.Http.Implementation
{
    /// <summary>
    /// A new HTTP header has been parsed.
    /// </summary>
    /// <see cref="HttpHeaderParser.HeaderParsed"/>
    public class HeaderEventArgs : EventArgs
    {
        /// <summary>
        /// Gets header value (unmodified)
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Gets header name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Set new header
        /// </summary>
        /// <param name="name">Header name</param>
        /// <param name="value">Header value</param>
        /// <remarks>Invoked by the parser so that we don't have to create a new EventArgs for each new parsed header.</remarks>
        public void Set(string name, string value)
        {
            if (name == null) throw new ArgumentNullException("name");
            Name = name;
            Value = value;
        }
    }
}