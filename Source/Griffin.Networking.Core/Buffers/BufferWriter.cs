using System;
using System.IO;

namespace Griffin.Networking.Buffers
{
    /// <summary>
    /// Can write information into a slice.
    /// </summary>
    public class BufferWriter : IBufferWriter
    {
        private IBufferSlice _slice;

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferWriter" /> class.
        /// </summary>
        /// <remarks>You must use <c>Assign()</c> before you can start using the writer (or by assigning a buffer using the other constructor)</remarks>
        public BufferWriter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferWriter" /> class.
        /// </summary>
        /// <param name="slice">The slice.</param>
        public BufferWriter(IBufferSlice slice)
        {
            if (slice == null) throw new ArgumentNullException("slice");
            _slice = slice;
        }

        /// <summary>
        /// Gets buffer that we are working with
        /// </summary>
        public byte[] Buffer
        {
            get
            {
                if (_slice == null)
                    throw new InvalidOperationException("No buffer is currently assigned.");

                return _slice.Buffer;
            }
        }

        #region IBufferWriter Members

        /// <summary>
        /// Gets current position in the buffer
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Gets number of bytes written to the buffer
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Gets our buffer capacity
        /// </summary>
        public int Capacity
        {
            get { return _slice == null ? 0 : _slice.Count; }
        }

        /// <summary>
        /// Write something to us from the specified buffer.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies <paramref name="count" /> bytes from <paramref name="buffer" /> to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        /// <exception cref="System.ArgumentNullException">buffer</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">offset;count;</exception>
        public void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            if (offset < 0 || offset >= buffer.Length)
                throw new ArgumentOutOfRangeException("offset", offset, "Must be 0 >= x < " + buffer.Length);
            if (count + Position >= _slice.Count)
                throw new ArgumentOutOfRangeException("count", count,
                                                      "Position + count must be less than " + _slice.Count);
            if (offset + count > buffer.Length)
                throw new ArgumentOutOfRangeException("offset", offset,
                                                      "Offset + Count must be less than " + buffer.Length);

            System.Buffer.BlockCopy(buffer, offset, _slice.Buffer, Position + _slice.Offset, count);
            Forward(count);
            Count += count;
        }

        /// <summary>
        /// Copy everything from the specified stream into this writer.
        /// </summary>
        /// <param name="stream">Stream to copy information from.</param>
        public void Copy(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            var bytesToCopy = (int) (stream.Length - stream.Position);
            if (bytesToCopy > Capacity - Position)
                throw new ArgumentOutOfRangeException("stream", stream,
                                                      "Stream.Length - Stream.Position (= bytes to copy) is larger then the amount of bytes left in the buffer slice.");

            while (true)
            {
                var bytesRead = stream.Read(_slice.Buffer, _slice.Offset + Position, bytesToCopy);
                Forward(bytesRead);
                bytesToCopy -= bytesRead;
                if (bytesToCopy == 0)
                    break;
            }
        }

        #endregion

        /// <summary>
        /// Move position forward in the buffer
        /// </summary>
        /// <param name="bytesToMove">Number of bytes to move forward</param>
        public void Forward(int bytesToMove)
        {
            Position += bytesToMove;
        }

        /// <summary>
        /// Assign a new slice to this writer
        /// </summary>
        /// <param name="slice">Slice to assign</param>
        /// <remarks>You must have called <see cref="Reset"/> first to release last buffer.</remarks>
        public void Assign(IBufferSlice slice)
        {
            if (slice == null) throw new ArgumentNullException("slice");
            if (_slice != null)
                throw new InvalidOperationException("You must reset the writer before assigning a new buffer.");

            Count = slice.Count;
            Position = slice.Offset;
            _slice = slice;
        }

        /// <summary>
        /// Dipose last buffer and reset indexes.
        /// </summary>
        public void Reset()
        {
            var disposable = _slice as IDisposable;
            if (disposable != null)
                disposable.Dispose();

            _slice = null;
            Count = 0;
            Position = 0;
        }
    }
}