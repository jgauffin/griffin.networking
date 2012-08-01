using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.Networking.Buffers
{
    /// <summary>
    /// Used to promote buffer reusage instead of allocating a new buffer each time
    /// </summary>
    public class BufferPool
    {
        private readonly int _bufferSize;
        private readonly int _capacity;
        private readonly LinkedList<byte[]> _buffers = new LinkedList<byte[]>();
        private int _handedOut;

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferPool"/> class.
        /// </summary>
        /// <param name="bufferSize">Size of each buffer.</param>
        /// <param name="count">Number of initial buffers.</param>
        /// <param name="capacity">Maximum amount of buffers.</param>
        public BufferPool(int bufferSize, int count, int capacity)
        {
            _bufferSize = bufferSize;
            _capacity = capacity;
            for (int i = 0; i < count; i++)
            {
                _buffers.AddLast(new byte[bufferSize]);
            }
        }

        /// <summary>
        /// Get a new buffer.
        /// </summary>
        /// <returns>A buffer</returns>
        public byte[] Pop()
        {
            lock (_buffers)
            {
                if (_buffers.Count > 0)
                {
                    var first = _buffers.First;
                    _buffers.RemoveFirst();
                    _handedOut++;
                    return first.Value;
                }

                if (_handedOut < _capacity)
                {
                    var buffer = new byte[_bufferSize];
                    _handedOut++;
                    return buffer;
                }

            }
            throw new InvalidOperationException(string.Format("Buffer pool at it's capacity of {0} items.", _capacity));
        }

        /// <summary>
        /// Get a buffer wrapped in a slice.
        /// </summary>
        /// <returns>Slice</returns>
        public BufferSlice PopSlice()
        {
            lock (_buffers)
            {
                if (_buffers.Count > 0)
                {
                    var first = _buffers.First.Value;
                    _buffers.RemoveFirst();
                    _handedOut++;
                    return new ReusableBufferSlice(this, first, 0, first.Length, 0);
                }

                if (_handedOut < _capacity)
                {
                    var buffer = new byte[_bufferSize];
                    _handedOut++;
                    return new ReusableBufferSlice(this, buffer, 0, buffer.Length, 0);
                }

            }
            throw new InvalidOperationException(string.Format("Buffer pool at it's capacity of {0} items.", _capacity));
        }

        /// <summary>
        /// return a buffer
        /// </summary>
        /// <param name="buffer">buffer to return</param>
        public void Push(byte[] buffer)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            if (buffer.Length != _bufferSize)
                throw new ArgumentException(
                    string.Format("Expected a buffer with the length {0}, but received one with {1}.", _bufferSize,
                                  buffer.Length));

            lock (_buffers)
            {
                _buffers.AddLast(buffer);
                --_handedOut;
            }
        }
    }

}
