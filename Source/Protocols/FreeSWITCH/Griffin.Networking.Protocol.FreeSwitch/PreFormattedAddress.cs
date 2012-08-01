namespace Griffin.Networking.Protocol.FreeSwitch
{
    /// <summary>
    /// end point addresses has already been formatted per freeswitch syntax.
    /// </summary>
    public class PreFormattedAddress : IEndpointAddress, ICallerIdProvider
    {
        /// <summary>
        /// Gets valid freeswitch endpoint address
        /// </summary>
        public string Address { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PreFormattedAddress"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        public PreFormattedAddress(string address)
        {
            Address = address;
        }

        /// <summary>
        /// Format the address as a string which could be dialed using the "originate" or "bridge" commands
        /// </summary>
        /// <returns>Properly formatted string</returns>
        public string ToDialString()
        {
            return Address;
        }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string CallerIdName { get; set; }

        /// <summary>
        /// Gets or sets the number, must be formatted per usage area (for instance per the trunk rules for outbound external calls)
        /// </summary>
        public string CallerIdNumber { get; set; }
    }
}