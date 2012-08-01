namespace Griffin.Networking.Protocol.FreeSwitch
{
    /// <summary>
    /// An extension registered in the dial plan
    /// </summary>
    public class ExtensionAddress : IEndpointAddress
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionAddress"/> class.
        /// </summary>
        /// <param name="extension">The extension.</param>
        public ExtensionAddress(string extension)
        {
            Value = extension;
        }

        /// <summary>
        /// Gets extension
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Format the address as a string which could be dialed using the "originate" or "bridge" commands
        /// </summary>
        /// <returns>Properly formatted string</returns>
        public string ToDialString()
        {
            return Value;
        }
    }
}