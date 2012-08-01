using System;

namespace Griffin.Networking.Protocol.FreeSwitch
{
    /// <summary>
    /// Dialing an application
    /// </summary>
    public class ApplicationDestination : IEndpointAddress
    {
        private readonly string _applicationName;
        private readonly string[] _arguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDestination"/> class.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="arguments">The arguments.</param>
        public ApplicationDestination(string applicationName, params string[] arguments)
        {
            if (applicationName == null) throw new ArgumentNullException("applicationName");
            if (arguments == null) throw new ArgumentNullException("arguments");
            _applicationName = applicationName;
            _arguments = arguments;
        }

        #region IEndpointAddress Members

        /// <summary>
        /// Format the address as a string which could be dialed using the "originate" or "bridge" commands
        /// </summary>
        /// <returns>Properly formatted string</returns>
        public string ToDialString()
        {
            return string.Format("&{0}({1})", _applicationName, string.Join(",", _arguments));
        }

        #endregion
    }
}