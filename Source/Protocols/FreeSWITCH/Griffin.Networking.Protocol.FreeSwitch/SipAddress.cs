namespace Griffin.Networking.Protocol.FreeSwitch
{
    /// <summary>
    /// A sip address
    /// </summary>
    public class SipAddress : IEndpointAddress
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SipAddress"/> class.
        /// </summary>
        /// <param name="domain">The domain.</param>
        /// <param name="extension">The extension.</param>
        public SipAddress(string domain, string extension)
        {
            Domain = domain;
            Extension = extension;
        }

        public string Extension { get; set; }

        public string Domain { get; set; }

        #region IEndpointAddress Members

        /// <summary>
        /// Format the address as a string which could be dialed using the "originate" or "bridge" commands
        /// </summary>
        /// <returns>Properly formatted string</returns>
        public string ToDialString()
        {
            return Extension + "@" + Domain;
        }

        #endregion

        public static SipAddress Parse(string fullAddress)
        {
            var parts = fullAddress.Split('@');
            if (parts.Length == 2)
            {
                return new SipAddress(parts[1], parts[0]);
            }
            return null;
        }
    }
}