using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.Networking.Protocol.Basic
{
    /// <summary>
    /// Must be implemented by all messages that are going to be sent using the Basic protocol.
    /// </summary>
    /// <remarks>The interface is used to be able to generate metadata. The metadata is in turn used to
    /// be able to produce a service documentation which can be passed to your clients.</remarks>
    public interface IBasicMessage
    {
    }
}
