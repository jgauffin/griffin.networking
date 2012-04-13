using System;
using System.IO;

namespace Griffin.Networking.Buffers
{
    /// <summary>
    /// A stream which supports peeking (lookforward)
    /// </summary>
    public class PeekableStream : Stream, IPeekable
    {
        private readonly BufferSlice _slice;

        /// <summary>
        /// Initializes a new instance of the <see cref="PeekableStream"/> class.
        /// </summary>
        /// <param name="slice">Slice used by this peekable stream</param>
        public PeekableStream(BufferSlice slice)
        {
            if (slice == null) throw new ArgumentNullException("slice");
            _slice = slice;
        }

        /// <summary>
        /// Gets allocated size.
        /// </summary>
        public int Capacity
        {
            get { return _slice.Capacity; }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PeekableStream"/> class.
        /// </summary>
        /// <param name="buffer">Buffer used by the stream.</param>
        /// <param name="offset">Offset in buffer that is assigned to this stream</param>
        /// <param name="count">Number of assigned bytes.</param>
        /// <param name="usedCount">Number of bytes which is used in the assigned slice.</param>
        /// <example>
        /// <code>
        /// var buffer = new byte[65536];
        /// 
        /// // write info to the buffer (fake a receive)
        /// var info = Encoding.ASCII.GetBytes("Hello world");
        /// Buffer.BlockCopy(buffer, 32768, info, 0, info.Length);
        /// 
        /// // we have been assigned a slice from offset 32768 and the slice is 32768 bytes long. 
        /// // info.length bytes have been written to the slice.
        /// var stream = new PeekableStream(buffer, 32768, 32768,  info.Length);
        /// </code>
        /// </example>
        public PeekableStream(byte[] buffer, int offset, int count, int usedCount)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            _slice = new BufferSlice(buffer, offset, count, usedCount);
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
            get { return true; }
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
            get { return _slice.Count; }
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
            get { return _slice.Position - _slice.StartOffset; }
            set
            {
                _slice.Position = (int)value + _slice.StartOffset;
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
            return _slice.RemainingLength > 0 ? (char)_slice.Buffer[_slice.Position + 1] : char.MinValue;
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
            if (origin == SeekOrigin.Begin)
                _slice.Position = _slice.StartOffset + (int)offset;
            else if (origin == SeekOrigin.Current)
                _slice.Position += (int)offset;
            else
                _slice.Position = _slice.Count - (int)offset;

            return _slice.Position - _slice.StartOffset;
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
            var bytesToRead = count > _slice.RemainingLength ? _slice.RemainingLength : count;
            Buffer.BlockCopy(_slice.Buffer, _slice.Position, buffer, offset, bytesToRead);
            _slice.Position += bytesToRead;
            return bytesToRead;
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
            if (count > _slice.RemainingCapacity)
                throw new InvalidOperationException("Allocated buffer is not large enough");

            Buffer.BlockCopy(buffer, offset, _slice.Buffer, _slice.Position, count);
            _slice.Position += count;
            _slice.Count += count;
        }
    }
}