using System;
using System.Collections.Concurrent;

namespace Griffin.Networking.Buffers.Reusable
{
    /// <summary>
    /// Another buffer implementation (experimental)
    /// </summary>
    public class ByteBufferPool : IBufferRecycler
    {
        private readonly byte[] _buffer;
        private readonly ConcurrentQueue<int> _bufferIndexes = new ConcurrentQueue<int>();
        private readonly int _bufferSize;
        private readonly int _capacity;

        /// <summary>
        /// Initializes a new instance of the <see cref="ByteBufferPool"/> class.
        /// </summary>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <param name="capacity">The capacity.</param>
        public ByteBufferPool(int bufferSize, int capacity)
        {
            _bufferSize = bufferSize;
            _capacity = capacity;
            _buffer = new byte[_capacity*_bufferSize];

            var index = 0;
            while (capacity > 0)
            {
                _bufferIndexes.Enqueue(index);
                index += _bufferSize;
                capacity--;
            }
        }

        #region IBufferRecycler Members

        void IBufferRecycler.Recycle(BufferSlize slize)
        {
            _bufferIndexes.Enqueue(slize.Offset);
        }

        #endregion

        /// <summary>
        /// Return a buffer
        /// </summary>
        /// <returns></returns>
        public BufferSlize Pop()
        {
            int index;
            if (!_bufferIndexes.TryDequeue(out index))
                throw new InvalidOperationException(string.Format("Buffer pool ({0}/{1}) is empty.", _bufferSize,
                                                                  _capacity));

            return new BufferSlize(this, _buffer, index, _bufferSize);
        }
    }
}