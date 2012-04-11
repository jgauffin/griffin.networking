using System.Collections.Generic;

namespace Griffin.Networking.Http.Protocol 
{
    /// <summary>
    /// Collection of headers.
    /// </summary>
    public interface IHeaderCollection : IEnumerable<IHeader>
    {
        /// <summary>
        /// Gets a header
        /// </summary>
        /// <param name="name">header name.</param>
        /// <returns>value if found; otherwise <c>null</c>.</returns>
        IHeader this[string name] { get; }
    }
}