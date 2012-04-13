using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Griffin.Networking.Buffers
{
    class BufferPoolStream2 : Stream
    {
        private readonly BufferPool _bufferPool;
        bool _canWrite;
        int _capacity;
        int _length;
        byte[] _buffer;
        int _initialIndex;
        bool _streamClosed;
        int _position;
        private BufferSlice _slice;
        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            _bufferPool.Push(_buffer);
            _disposed = true;
            base.Dispose(disposing);
        }

        public BufferPoolStream2(BufferPool bufferPool)
        {
            _bufferPool = bufferPool;
            _canWrite = true;
            _slice = bufferPool.PopSlice();

            _buffer = _slice.Buffer;
            _capacity = _slice.Capacity;
            _length = _slice.Count;
            _position = _slice.Position;
            _initialIndex = _slice.StartOffset;
        }

        void CheckIfClosedThrow()
        {
            if(!_disposed)
                throw new InvalidOperationException("Stream have been closed.");

            if (_streamClosed)
                throw new ObjectDisposedException("MemoryStream");
        }

        public override bool CanRead
        {
            get { return !_streamClosed; }
        }

        public override bool CanSeek
        {
            get { return !_streamClosed; }
        }

        public override bool CanWrite
        {
            get { return (!_streamClosed && _canWrite); }
        }

        public virtual int Capacity
        {
            get
            {
                CheckIfClosedThrow();
                return _capacity - _initialIndex;
            }

            set
            {
                CheckIfClosedThrow();
                throw new NotSupportedException("Cannot expand this BufferPoolStream");
            }
        }

        public override long Length
        {
            get
            {
                // LAMESPEC: The spec says to throw an IOException if the
                // stream is closed and an ObjectDisposedException if
                // "methods were called after the stream was closed".  What
                // is the difference?

                CheckIfClosedThrow();

                // This is ok for MemoryStreamTest.ConstructorFive
                return _length - _initialIndex;
            }
        }

        public override long Position
        {
            get
            {
                CheckIfClosedThrow();
                return _position - _initialIndex;
            }

            set
            {
                CheckIfClosedThrow();
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value",
                                "Position cannot be negative");

                if (value > Int32.MaxValue)
                    throw new ArgumentOutOfRangeException("value",
                    "Position must be non-negative and less than 2^31 - 1 - origin");

                _position = _initialIndex + (int)value;
            }
        }

        public override void Close()
        {
            _streamClosed = true;
            
        }

        public override void Flush()
        {
            // Do nothing
        }

        public override int Read([In, Out] byte[] buffer, int offset, int count)
        {
            CheckIfClosedThrow();

            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (offset < 0 || count < 0)
                throw new ArgumentOutOfRangeException("offset", "offset or count less than zero.");
            if (buffer.Length - offset < count)
                throw new ArgumentException("The size of the buffer is less than offset + count.", "count");

            if (_position >= _length || count == 0)
                return 0;

            if (_position > _length - count)
                count = _length - _position;

            Buffer.BlockCopy(_buffer, _position, buffer, offset, count);
            _position += count;
            return count;
        }

        public override int ReadByte()
        {
            CheckIfClosedThrow();
            if (_position >= _length)
                return -1;

            return _buffer[_position++];
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            CheckIfClosedThrow();
            if (offset > (long)Int32.MaxValue)
                throw new ArgumentOutOfRangeException("Offset out of range. " + offset);

            int pos;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    if (offset < 0)
                        throw new IOException("Attempted to seek before start of MemoryStream.");
                    pos = _initialIndex;
                    break;
                case SeekOrigin.Current:
                    pos = _position;
                    break;
                case SeekOrigin.End:
                    pos = _length;
                    break;
                default:
                    throw new ArgumentException("origin", "Invalid SeekOrigin");
            }

            pos += (int)offset;
            if (pos < _initialIndex)
                throw new IOException("Attempted to seek before start of Stream.");

            _position = pos;
            return _position;
        }

        public override void SetLength(long value)
        {
                throw new NotSupportedException("Expanding this Stream is not supported");

        }


        public override void Write(byte[] buffer, int offset, int count)
        {
            CheckIfClosedThrow();
            if (!_canWrite)
                throw new NotSupportedException("Cannot write to this stream.");
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (offset < 0 || count < 0)
                throw new ArgumentOutOfRangeException();
            if (buffer.Length - offset < count)
                throw new ArgumentException("The size of the buffer is less than offset + count.", "count");

            // reordered to avoid possible integer overflow
            if ((_position - _initialIndex) > _capacity - count)
                throw new ArgumentOutOfRangeException("count", "");

            Buffer.BlockCopy(buffer, offset, _buffer, _position, count);
            _position += count;
            if (_position >= _length)
                _length = _position;
        }

        public override void WriteByte(byte value)
        {
            CheckIfClosedThrow();
            if (!_canWrite)
                throw new NotSupportedException("Cannot write to this stream.");

            if (_position >= _capacity)
                throw new ArgumentOutOfRangeException("value", "Buffer overflow");

            if (_position >= _length)
                _length = _position + 1;

            _buffer[_position++] = value;
        }

        public virtual void WriteTo(Stream stream)
        {
            CheckIfClosedThrow();

            if (stream == null)
                throw new ArgumentNullException("stream");

            stream.Write(_buffer, _initialIndex, _length - _initialIndex);
        }
    }
}
