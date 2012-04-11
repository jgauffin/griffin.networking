using System;
using Griffin.Networking.Buffers;

namespace Griffin.Networking.Messages
{
    /// <summary>
    /// Send a message in the channel (through the pipeline)
    /// </summary>
    public class SendMessage : IPipelineMessage
    {
        public SendMessage(IBufferSlice slice)
        {
            if (slice == null)
                throw new ArgumentNullException("slice");

            BufferSlice = slice;
        }
        
        /// <summary>
        /// Gets buffer slice to send.
        /// </summary>
        public IBufferSlice BufferSlice { get; private set; }
    }
}