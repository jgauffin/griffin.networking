using System;
using Griffin.Networking.Buffers;

namespace Griffin.Networking.Clients
{
    /// <summary>
    /// We've received something from the client.
    /// </summary>
    public class ReceivedBufferEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivedBufferEventArgs" /> class.
        /// </summary>
        /// <param name="bufferReader">The buffer reader.</param>
        public ReceivedBufferEventArgs(IBufferReader bufferReader)
        {
            BufferReader = bufferReader;
        }

        /// <summary>
        /// Gets reader which we can get bytes from
        /// </summary>
        public IBufferReader BufferReader { get; private set; }
    }
}