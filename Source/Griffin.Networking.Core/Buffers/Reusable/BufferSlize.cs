using System;

namespace Griffin.Networking.Buffers.Reusable
{
    /// <summary>
    /// A slice of a larger buffer.
    /// </summary>
    public class BufferSlize : IDisposable
    {
        private IBufferRecycler _pool;

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferSlize"/> class.
        /// </summary>
        /// <param name="pool">The pool.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        public BufferSlize(IBufferRecycler pool, byte[] buffer, int offset, int length)
        {
            _pool = pool;
            Buffer = buffer;
            Offset = offset;
            Length = length;
        }

        /// <summary>
        /// Gets buffer that this slice is part of
        /// </summary>
        public byte[] Buffer { get; private set; }

        /// <summary>
        /// Gets current offset in the buffer
        /// </summary>
        public int Offset { get; private set; }

        /// <summary>
        /// Gets our allocated length
        /// </summary>
        public int Length { get; private set; }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_pool == null)
                return;
            _pool.Recycle(this);
            _pool = null;
        }

        #endregion
    }
}