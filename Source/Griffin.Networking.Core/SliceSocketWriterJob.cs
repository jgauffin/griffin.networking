using System;
using System.Net.Sockets;
using Griffin.Networking.Buffers;

namespace Griffin.Networking
{
    /// <summary>
    /// Writes a buffer slice to the socket.
    /// </summary>
    /// <remarks>Make sure that the entire slice fits the Sockets internal buffer.</remarks>
    public class SliceSocketWriterJob : ISocketWriterJob
    {
        private readonly int _length;
        private int _bytesLeft;
        private int _offset;
        private IBufferSlice _slice;

        /// <summary>
        /// Initializes a new instance of the <see cref="SliceSocketWriterJob" /> class.
        /// </summary>
        /// <param name="slice">Slice to send.</param>
        /// <param name="count">Number of bytes in the slice.</param>
        public SliceSocketWriterJob(IBufferSlice slice, int count)
        {
            if (slice == null) throw new ArgumentNullException("slice");
            _slice = slice;
            _offset = _slice.Offset;
            _bytesLeft = count;
            _length = count;
        }

        #region ISocketWriterJob Members

        /// <summary>
        /// Write stuff to our args.
        /// </summary>
        /// <param name="args">Args used when sending bytes to the socket</param>
        public void Write(SocketAsyncEventArgs args)
        {
            args.SetBuffer(_slice.Buffer, _offset, _bytesLeft);
        }

        /// <summary>
        /// The async write has been completed
        /// </summary>
        /// <param name="bytes">Number of bytes that was sent</param>
        /// <returns>
        ///   <c>true</c> if everything was sent; otherwise <c>false</c>.
        /// </returns>
        public bool WriteCompleted(int bytes)
        {
            _bytesLeft -= bytes;
            _offset += bytes;
            return _bytesLeft == 0;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            var disposable = _slice as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
                _slice = null;
            }
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Slice at offset {0} with {1}/{2} bytes.", _slice.Offset, _bytesLeft, _length);
        }
    }
}