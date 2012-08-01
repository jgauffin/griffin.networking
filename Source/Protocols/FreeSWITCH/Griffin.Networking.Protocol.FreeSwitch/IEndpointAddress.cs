using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.Networking.Protocol.FreeSwitch
{
    /// <summary>
    /// An address for an FreeSWITCH end point
    /// </summary>
    /// <remarks>Can be a SIP address, an sofia address or event an application</remarks>
    public interface IEndpointAddress
    {
        /// <summary>
        /// Format the address as a string which could be dialed using the "originate" or "bridge" commands
        /// </summary>
        /// <returns>Properly formatted string</returns>
        string ToDialString();
    }
}
