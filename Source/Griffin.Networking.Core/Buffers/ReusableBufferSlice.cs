using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.Networking.Buffers
{
    /// <summary>
    /// Gives back the buffer to the pool when disposed.
    /// </summary>
    public class ReusableBufferSlice : BufferSlice, IDisposable
    {
        private BufferPool _pool;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReusableBufferSlice"/> class.
        /// </summary>
        /// <param name="pool">Pool that the buffer should be released to.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="startOffset">Offset in buffer where the slice starts.</param>
        /// <param name="capacity">Number of bytes allocated for this slice.</param>
        /// <param name="count">Number of bytes written to the buffer (if any)</param>
        public ReusableBufferSlice(BufferPool pool, byte[] buffer, int startOffset, int capacity, int count)
            : base(buffer, startOffset, capacity, count)
        {
            _pool = pool;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (_pool == null)
                return;

            _pool.Push(Buffer);
            _pool = null;
        }
    }
}
