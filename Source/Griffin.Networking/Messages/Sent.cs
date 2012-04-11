using System;
using Griffin.Networking.Buffers;

namespace Griffin.Networking.Messages
{
    /// <summary>
    /// A message have been sent by the channel
    /// </summary>
    public class Sent : IPipelineMessage
    {
        public BufferSlice BufferSlice { get; private set; }

        public Sent(BufferSlice bufferSlice)
        {
            if (bufferSlice == null)
                throw new ArgumentNullException("bufferSlice");

            BufferSlice = bufferSlice;
        }
    }
}