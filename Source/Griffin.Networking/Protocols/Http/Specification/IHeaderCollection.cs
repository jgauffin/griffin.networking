using System.Collections.Generic;

namespace Griffin.Networking.Protocols.Http
{
    /// <summary>
    /// Collection of headers.
    /// </summary>
    public interface IHeaderCollection : IEnumerable<KeyValuePair<string, string>>
    {
        /// <summary>
        /// Gets a header
        /// </summary>
        /// <param name="name">header name.</param>
        /// <returns>value if found; otherwise <c>null</c>.</returns>
        string this[string name] { get; set; }
    }
}