using System;
using System.IO;

namespace Griffin.Networking.Buffers
{
    /// <summary>
    /// Not yet completed.
    /// </summary>
    public class CircularStream : Stream, IPeekable
    {
        private readonly byte[] _buffer;
        private readonly int _capacity;
        private int _count;
        private readonly int _startOffset;
        private long _position; // 0 based "virtual" position

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularStream"/> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="capacity">The capacity.</param>
        public CircularStream(byte[] buffer, int offset, int capacity)
        {
            _buffer = buffer;
            _startOffset = offset;
            _capacity = capacity;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularStream"/> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="capacity">The capacity.</param>
        /// <param name="writtenCount">The written count.</param>
        public CircularStream(byte[] buffer, int offset, int capacity, int writtenCount)
        {
            _buffer = buffer;
            _startOffset = offset;
            _capacity = capacity;
            _count = writtenCount;
        }

        private int EndOffset
        {
            get { return _startOffset + _capacity; }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <returns>
        /// true if the stream supports reading; otherwise, false.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public override bool CanRead
        {
            get { return true; }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <returns>
        /// true if the stream supports seeking; otherwise, false.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public override bool CanSeek
        {
            get { return true; }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <returns>
        /// true if the stream supports writing; otherwise, false.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public override bool CanWrite
        {
            get { return _count < _capacity; }
        }

        /// <summary>
        /// When overridden in a derived class, gets the length in bytes of the stream.
        /// </summary>
        /// <returns>
        /// A long value representing the length of the stream in bytes.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">A class derived from Stream does not support seeking. 
        ///                 </exception><exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. 
        ///                 </exception><filterpriority>1</filterpriority>
        public override long Length
        {
            get { return _count; }
        }

        /// <summary>
        /// Clears the specified offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        public void Clear(int offset, int count)
        {
            
        }

        /// <summary>
        /// When overridden in a derived class, gets or sets the position within the current stream.
        /// </summary>
        /// <returns>
        /// The current position within the stream.
        /// </returns>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. 
        ///                 </exception><exception cref="T:System.NotSupportedException">The stream does not support seeking. 
        ///                 </exception><exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. 
        ///                 </exception><filterpriority>1</filterpriority>
        public override long Position
        {
            get
            {
                return _position;
            }
            set
            {
                if (value > _capacity)
                {
                    throw new ArgumentOutOfRangeException(
                        string.Format("'{0}' is larger than the allocated buffer of '{1}'", value, _capacity));
                }

                // 4 5 6 7 8
                //     Y        Position += 4 => 6 (offset) + 4 = 10 => 9 - 5 (capacity) = 5
                //              
                var newPosition = _startOffset + value;
                if (newPosition > EndOffset)
                    newPosition -= _capacity;

                _position = newPosition;

            }
        }

        #region IPeekable Members

        /// <summary>
        /// Peek at the next byte in the sequence.
        /// </summary>
        /// <returns>
        /// Char if not EOF; otherwise <see cref="char.MinValue"/>
        /// </returns>
        public char Peek()
        {
            if (_position >= _count)
                return char.MinValue;

            var peekPos = _position + 1;
            if (_startOffset + _startOffset > EndOffset)
                return (char)_buffer[_startOffset];

            return (char)_buffer[peekPos];
        }

        #endregion

        /// <summary>
        /// When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. 
        ///                 </exception><filterpriority>2</filterpriority>
        public override void Flush()
        {
        }

        /// <summary>
        /// When overridden in a derived class, sets the position within the current stream.
        /// </summary>
        /// <returns>
        /// The new position within the current stream.
        /// </returns>
        /// <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter. 
        ///                 </param><param name="origin">A value of type <see cref="T:System.IO.SeekOrigin"/> indicating the reference point used to obtain the new position. 
        ///                 </param><exception cref="T:System.IO.IOException">An I/O error occurs. 
        ///                 </exception><exception cref="T:System.NotSupportedException">The stream does not support seeking, such as if the stream is constructed from a pipe or console output. 
        ///                 </exception><exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. 
        ///                 </exception><filterpriority>1</filterpriority>
        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = _startOffset + (int)offset;
                    break;
                case SeekOrigin.Current:
                    Position += (int)offset;
                    break;
                default:
                    Position = _count - (int)offset;
                    break;
            }

            return Position;
        }

        /// <summary>
        /// When overridden in a derived class, sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes. 
        ///                 </param><exception cref="T:System.IO.IOException">An I/O error occurs. 
        ///                 </exception><exception cref="T:System.NotSupportedException">The stream does not support both writing and seeking, such as if the stream is constructed from a pipe or console output. 
        ///                 </exception><exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. 
        ///                 </exception><filterpriority>2</filterpriority>
        public override void SetLength(long value)
        {
            throw new NotSupportedException("Buffer size is read only");
        }

        /// <summary>
        /// When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by the bytes read from the current source. 
        ///                 </param><param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read from the current stream. 
        ///                 </param><param name="count">The maximum number of bytes to be read from the current stream. 
        ///                 </param><exception cref="T:System.ArgumentException">The sum of <paramref name="offset"/> and <paramref name="count"/> is larger than the buffer length. 
        ///                 </exception><exception cref="T:System.ArgumentNullException"><paramref name="buffer"/> is null. 
        ///                 </exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="offset"/> or <paramref name="count"/> is negative. 
        ///                 </exception><exception cref="T:System.IO.IOException">An I/O error occurs. 
        ///                 </exception><exception cref="T:System.NotSupportedException">The stream does not support reading. 
        ///                 </exception><exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. 
        ///                 </exception><filterpriority>1</filterpriority>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset");
            if (count < 0)
                throw new ArgumentOutOfRangeException("offset");
            if (offset + _startOffset >= EndOffset)
                throw new ArgumentOutOfRangeException(
                    string.Format("Offset {0} is larger than stream size of {1}.", offset, _count));
            if (offset + count > buffer.Length)
                throw new ArgumentException(
                    string.Format("Offset {0} + Count {1} is larger than buffer size of {2}.", offset, count, buffer.Length));

            var realPosition = _position + _startOffset;
            if (count > count - Position)
                count = count - (int)Position;

            // split copy
            if (realPosition + count > EndOffset)
            {
                var firstAmount = EndOffset - realPosition;
                Buffer.BlockCopy(_buffer, (int)_position, buffer, offset, (int)firstAmount);
                var secondAmount = count - firstAmount;
                Buffer.BlockCopy(_buffer, 0, _buffer, offset + (int)firstAmount, (int)secondAmount);
            }
            else
            {
                Buffer.BlockCopy(_buffer, (int)realPosition, buffer, offset, count);
            }

            return 0;
        }

        /// <summary>
        /// When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies <paramref name="count"/> bytes from <paramref name="buffer"/> to the current stream. 
        ///                 </param><param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin copying bytes to the current stream. 
        ///                 </param><param name="count">The number of bytes to be written to the current stream. 
        ///                 </param><exception cref="T:System.ArgumentException">The sum of <paramref name="offset"/> and <paramref name="count"/> is greater than the buffer length. 
        ///                 </exception><exception cref="T:System.ArgumentNullException"><paramref name="buffer"/> is null. 
        ///                 </exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="offset"/> or <paramref name="count"/> is negative. 
        ///                 </exception><exception cref="T:System.IO.IOException">An I/O error occurs. 
        ///                 </exception><exception cref="T:System.NotSupportedException">The stream does not support writing. 
        ///                 </exception><exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. 
        ///                 </exception><filterpriority>1</filterpriority>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (offset + count > buffer.Length)
                throw new ArgumentOutOfRangeException("offset");
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (count > _capacity - count)
                throw new InvalidOperationException(string.Format("Want to write {0} bytes, only {1} is left of the capacity of {2}", count, _capacity - _count, _capacity));

            /*
            // split copy
            if (_startOffset + _position > EndOffset)
            {
                var overflow = EndOffset - _startOffset - _position;
                var firstCopy = _startOffset + _position - overflow;
                Buffer.BlockCopy(_buffer, (int)_position, buffer, offset, firstCopy);
                Buffer.BlockCopy(_buffer, );
            }
            _count += count;
            if ()
            Buffer.BlockCopy(buffer, offset, _slice.Buffer, _slice.Offset, count);
            _slice.Offset += count;
            */
        }
    }
}