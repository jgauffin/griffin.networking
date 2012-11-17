using Griffin.Networking.Http.Protocol;

namespace Griffin.Networking.Http.Implementation
{
    internal class HttpCookie : IHttpCookie
    {
        #region IHttpCookie Members

        /// <summary>
        /// Gets the cookie identifier.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets value. 
        /// </summary>
        /// <remarks>
        /// Set to <c>null</c> to remove cookie.
        /// </remarks>
        public string Value { get; set; }

        #endregion
    }
}