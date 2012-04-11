using System;
using Griffin.Networking.Http.Protocol;

namespace Griffin.Networking.Http.Implementation
{
    internal class HttpHeader : IHeader
    {
        public HttpHeader(string name, string value)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (value == null) throw new ArgumentNullException("value");
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Gets header name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets value as it would be sent back to client.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Does a case insensitive compare with the specified value
        /// </summary>
        /// <param name="value">Value to compare our value with</param>
        /// <returns>true if equal; otherwase false;</returns>
        public bool Is(string value)
        {
            return Value.Equals(value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks if the header has the specified parameter
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <returns>true if equal; otherwase false;</returns>
        public bool HasParameter(string name)
        {
            return false;
        }
    }
}