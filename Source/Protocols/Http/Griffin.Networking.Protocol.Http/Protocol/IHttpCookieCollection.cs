using System.Collections.Generic;

namespace Griffin.Networking.Http.Specification
{
    /// <summary>
    /// Collection of cookies
    /// </summary>
    /// <typeparam name="T">Type of cookie</typeparam>
    public interface IHttpCookieCollection<out T> : IEnumerable<T> where T : IHttpCookie
    {
        /// <summary>
        /// Gets the count of cookies in the collection.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets the cookie of a given identifier (<c>null</c> if not existing).
        /// </summary>
        T this[string id] { get; }

        /// <summary>
        /// Remove all cookies.
        /// </summary>
        void Clear();

        /// <summary>
        /// Remove a cookie from the collection.
        /// </summary>
        /// <param name="cookieName">Name of cookie.</param>
        void Remove(string cookieName);
    }
}