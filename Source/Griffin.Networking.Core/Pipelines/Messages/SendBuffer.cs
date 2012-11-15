using System;

namespace Griffin.Networking.Pipelines.Messages
{
    /// <summary>
    /// Send a byte[] buffer
    /// </summary>
    public class SendBuffer : IPipelineMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendBuffer"/> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">Offset in buffer.</param>
        /// <param name="count">Number of bytes to send.</param>
        public SendBuffer(byte[] buffer, int offset, int count)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            if (buffer.Length < count)
                throw new ArgumentOutOfRangeException("count",
                                                      string.Format("Count {0} is larger than buffer length {1}.", count,
                                                                    buffer.Length));
            if (buffer.Length < offset + count)
                throw new ArgumentException(string.Format("Offset+Count ({0}+{1}) is past end of buffer.", offset, count));

            Buffer = buffer;
            Offset = offset;
            Count = count;
        }

        /// <summary>
        /// Gets buffer
        /// </summary>
        public byte[] Buffer { get; protected set; }

        /// <summary>
        /// Gets our starting offset
        /// </summary>
        public int Offset { get; protected set; }

        /// <summary>
        /// Gets number of bytes to send
        /// </summary>
        public int Count { get; protected set; }
    }
}