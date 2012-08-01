using System;
using Griffin.Networking.SimpleBinary.Handlers;

namespace Griffin.Networking.SimpleBinary.Messages
{
    /// <summary>
    /// We've received a packet which <see cref="ContentDecoder"/> has decoded.
    /// </summary>
    public class ReceivedPacket : IPipelineMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivedPacket"/> class.
        /// </summary>
        /// <param name="packet">The packet.</param>
        public ReceivedPacket(object packet)
        {
            if (packet == null) throw new ArgumentNullException("packet");
            Packet = packet;
        }

        /// <summary>
        /// Gets decoded object.
        /// </summary>
        public object Packet { get; private set; }
    }
}