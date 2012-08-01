namespace Griffin.Networking.Http.Protocol
{
    /// <summary>
    /// Header in a message
    /// </summary>
    /// <remarks>
    /// Important! Each header should override ToString() 
    /// and return it's data correctly formatted as a HTTP header value.
    /// </remarks>
    public interface IHeaderItem
    {
        /// <summary>
        /// Gets header name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets value as it would be sent back to client.
        /// </summary>
        string Value { get; }

        /// <summary>
        /// Does a case insensitive compare with the specified value
        /// </summary>
        /// <param name="value">Value to compare our value with</param>
        /// <returns>true if equal; otherwase false;</returns>
        bool Is(string value);

        /// <summary>
        /// Checks if the header has the specified parameter
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <returns>true if equal; otherwase false;</returns>
        bool HasParameter(string name);

        /// <summary>
        /// Get a parameter from the header
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string GetParameter(string name);
    }
}