using System;
using Griffin.Networking.Buffers;

namespace Griffin.Networking.Messages
{
    /// <summary>
    /// Send a slice 
    /// </summary>
    public class SendSlice : IPipelineMessage
    {
        public SendSlice(IBufferSlice slice)
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