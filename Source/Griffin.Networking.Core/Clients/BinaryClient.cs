using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Networking.Buffers;

namespace Griffin.Networking.Clients
{
    /// <summary>
    /// Represents a client connection which is only transfering byte[] buffers.
    /// </summary>
    /// <remarks>Gives you fill control of everything sent, including the buffers used. Typically used to gain
    /// speed, sending large objects or streaming.</remarks>
    public class BinaryClient : ClientBase
    {
        /// <summary>
        /// We've received something from the other end
        /// </summary>
        /// <param name="buffer">Buffer containing the received bytes</param>
        /// <param name="bytesRead">Amount of bytes that we received</param>
        /// <remarks>
        /// You have to handle all bytes, anything left will be discarded.
        /// </remarks>
        protected override void OnReceived(IBufferSlice buffer, int bytesRead)
        {
            Received(this, new ReceivedBufferEventArgs(new SliceStream(buffer, bytesRead) ));
        }

        /// <summary>
        /// We received something from the server
        /// </summary>
        public event EventHandler<ReceivedBufferEventArgs> Received = delegate { };
    }
}
