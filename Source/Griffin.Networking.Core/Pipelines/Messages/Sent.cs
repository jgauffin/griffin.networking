using System;
using Griffin.Networking.Buffers;

namespace Griffin.Networking.Pipelines.Messages
{
    /// <summary>
    /// A message have been sent by the channel
    /// </summary>
    public class Sent : IPipelineMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sent"/> class.
        /// </summary>
        /// <param name="bufferSlice">The buffer slice.</param>
        public Sent(BufferSlice bufferSlice)
        {
            if (bufferSlice == null)
                throw new ArgumentNullException("bufferSlice");

            BufferSlice = bufferSlice;
        }

        /// <summary>
        /// Gets the buffer slice.
        /// </summary>
        public BufferSlice BufferSlice { get; private set; }
    }
}