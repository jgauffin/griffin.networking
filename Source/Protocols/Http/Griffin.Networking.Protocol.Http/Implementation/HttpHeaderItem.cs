using System;
using Griffin.Networking.Http.Protocol;

namespace Griffin.Networking.Http.Implementation
{
    internal class HttpHeaderItem : IHeaderItem
    {
        public HttpHeaderItem(string name, string value)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (value == null) throw new ArgumentNullException("value");
            Name = name;
            Value = value;
        }

        #region IHeaderItem Members

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

        /// <summary>
        /// Get a parameter from the header
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetParameter(string name)
        {
            return "";
        }

        #endregion

        public void AddValue(string value)
        {
            Value += ", " + value;
        }
    }
}