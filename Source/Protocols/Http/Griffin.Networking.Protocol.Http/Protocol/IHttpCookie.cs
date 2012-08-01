namespace Griffin.Networking.Http
{
    public interface IHttpCookie
    {
        /// <summary>
        /// Gets the cookie identifier.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets value. 
        /// </summary>
        /// <remarks>
        /// Set to <c>null</c> to remove cookie.
        /// </remarks>
        string Value { get; set; }
    }
}