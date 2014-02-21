using System;
using Griffin.Networking.Protocol.Http.Protocol;

namespace Griffin.Networking.Protocol.Http.Implementation
{
    /// <summary>
    /// Response cookies also have an expiration and the path that they are valid for.
    /// </summary>
    public class HttpResponseCookie : HttpCookie, IResponseCookie
    {
        #region IResponseCookie Members

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponseCookie" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public HttpResponseCookie(string name, string value) : base(name, value)
        {
        }

        /// <summary>
        /// Gets domain that the cookie is valid under
        /// </summary>
        /// <remarks><c>null</c> means not specified</remarks>
        public string Domain { get; set; }

        /// <summary>
        /// Gets when the cookie expires.
        /// </summary>
        /// <remarks><see cref="DateTime.MinValue"/> means that the cookie expires when the session do so.</remarks>
        public DateTime Expires { get; set; }

        /// <summary>
        /// Gets path that the cookie is valid under.
        /// </summary>
        /// <remarks><c>null</c> means not specified.</remarks>
        public string Path { get; set; }

        #endregion
    }
}