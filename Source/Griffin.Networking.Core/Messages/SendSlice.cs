using System;
using Griffin.Networking.Buffers;

namespace Griffin.Networking.Messages
{
    /// <summary>
    /// Send a slice 
    /// </summary>
    public class SendSlice : IPipelineMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendSlice"/> class.
        /// </summary>
        /// <param name="slice">The slice.</param>
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