using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.Networking.Buffers
{
    public class BufferPool
    {
        private readonly int _bufferSize;
        private readonly int _capacity;
        public LinkedList<byte[]> _buffers = new LinkedList<byte[]>();
        private int _handedOut;

        public BufferPool(int bufferSize, int count, int capacity)
        {
            _bufferSize = bufferSize;
            _capacity = capacity;
            for (int i = 0; i < count; i++)
            {
                _buffers.AddLast(new byte[bufferSize]);
            }
        }

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
