using System;

namespace Griffin.Networking.Buffers
{
    /// <summary>
    /// A slice of an larger buffer.
    /// </summary>
    public class BufferSlice : IBufferSlice
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BufferSlice" /> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">Where our slice starts.</param>
        /// <param name="count">Number of bytes in our slice.</param>
        public BufferSlice(byte[] buffer, int offset, int count)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            if (offset < 0 || offset >= buffer.Length)
                throw new ArgumentOutOfRangeException("offset", offset,
                                                      string.Format("Offset must be between 0 and {0}.",
                                                                    (buffer.Length - 1)));
            if (offset + count > buffer.Length)
                throw new ArgumentOutOfRangeException("count", count,
                                                      string.Format(
                                                          "offset+count can not be larger than the buffer size: {0}",
                                                          buffer.Length));

            Buffer = buffer;
            Offset = offset;
            Count = count;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferSlice" /> class.
        /// </summary>
        /// <param name="count">Number of bytes.</param>
        /// <remarks>Allocates a new buffer</remarks>
        public BufferSlice(int count)
        {
            Buffer = new byte[count];
            Offset = 0;
            Count = count;
        }

        #region IBufferSlice Members

        /// <summary>
        /// Gets buffer that the slice is in
        /// </summary>
        public byte[] Buffer { get; private set; }

        /// <summary>
        /// Gets offset for our slice
        /// </summary>
        public int Offset { get; private set; }

        /// <summary>
        /// Gets the number of bytes that our slice has.
        /// </summary>
        public int Count { get; private set; }

        #endregion
    }
}