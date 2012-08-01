using System;

namespace Griffin.Networking.Protocol.FreeSwitch
{
    /// <summary>
    /// Destination as a FreeSWITCH Sofia sip address.
    /// </summary>
    public class SofiaSipAddress : IEndpointAddress
    {
        private readonly string _profileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SofiaSipAddress"/> class.
        /// </summary>
        /// <param name="profileName">Profile (context) name.</param>
        /// <param name="userName">User name (and if needed, IP address / domain name).</param>
        public SofiaSipAddress(string profileName, string userName)
        {
            if (profileName == null) throw new ArgumentNullException("profileName");
            if (userName == null) throw new ArgumentNullException("userName");
            UserName = userName;
            _profileName = profileName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SofiaSipAddress"/> class.
        /// </summary>
        /// <param name="address">Another address.</param>
        public SofiaSipAddress(SipAddress address)
        {
            _profileName = address.Domain;
            UserName = address.Extension;
        }

        /// <summary>
        /// Gets users to dial
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// Parses the specified full address.
        /// </summary>
        /// <param name="fullAddress">The full address.</param>
        /// <returns></returns>
        public static SofiaSipAddress Parse(string fullAddress)
        {
            var parts = fullAddress.Split('@');
            if (parts.Length == 2)
                return new SofiaSipAddress(parts[1], parts[0]);

            parts = fullAddress.Split('/');
            return parts.Length == 3 ? new SofiaSipAddress(parts[1], parts[2]) : null;
        }

        /// <summary>
        /// Format the address as a string which could be dialed using the "originate" or "bridge" commands
        /// </summary>
        /// <returns>Properly formatted string</returns>
        public string ToDialString()
        {
            return "sofia/" + _profileName + "/" + UserName;
        }
    }
}