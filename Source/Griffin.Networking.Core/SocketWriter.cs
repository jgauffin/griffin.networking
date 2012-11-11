using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using Griffin.Networking.Buffers;

namespace Griffin.Networking
{
    /// <summary>
    /// Used to write information to a socket in a queued fashion.
    /// </summary>
    public class SocketWriter
    {
        private readonly SocketAsyncEventArgs _writeArgs = new SocketAsyncEventArgs();
        private readonly BufferWriter _writeBuffer = new BufferWriter();
        private readonly ConcurrentQueue<WrappedBuffer> _writeQueue = new ConcurrentQueue<WrappedBuffer>();
        private Socket _socket;

        public SocketWriter()
        {
            _writeArgs.Completed += OnWriteCompleted;
        }

        private void OnWriteCompleted(object sender, SocketAsyncEventArgs e)
        {
            HandleWriteCompleted(e.SocketError, e.BytesTransferred);
        }

        /// <summary>
        /// Assign socket which will be used for writing.
        /// </summary>
        /// <param name="socket"></param>
        public void Assign(Socket socket)
        {
            if (socket == null) throw new ArgumentNullException("socket");
            if (!socket.Connected)
                throw new InvalidOperationException("Socket must be connected.");

            _socket = socket;
        }

        /// <summary>
        /// Sends the specified slice.
        /// </summary>
        /// <param name="slice">An allocated buffer.</param>
        /// <param name="count">Number of bytes in the buffer.</param>
        /// <exception cref="System.ArgumentNullException">slice</exception>
        /// <exception cref="System.InvalidOperationException">Socket as not been Assign():ed.</exception>
        public void Send(IBufferSlice slice, int count)
        {
            if (slice == null) throw new ArgumentNullException("slice");
            if (_socket == null)
                throw new InvalidOperationException("Socket as not been Assign():ed.");

            _writeQueue.Enqueue(new WrappedBuffer(slice, count));
            lock (_writeBuffer)
            {
                if (_writeBuffer.Count == 0)
                    SendNextBuffer();
            }
        }

        private void HandleWriteCompleted(SocketError error, int bytesTransferred)
        {
            if (error == SocketError.Success)
            {
                lock (_writeBuffer)
                {
                    // did not transfer everything.
                    if (bytesTransferred < _writeBuffer.Count)
                    {
                        _writeBuffer.Forward(bytesTransferred);
                        _writeArgs.SetBuffer(_writeBuffer.Buffer, _writeBuffer.Position, _writeBuffer.Count);
                        _socket.SendAsync(_writeArgs);
                        return;
                    }

                    // Send next buffer if any
                    SendNextBuffer();
                }
            }
            else
            {
                HandleDisconnect(error);
            }
        }

        private void HandleDisconnect(SocketError error)
        {
            Reset();
            Disconnected(this, new DisconnectEventArgs(error));
        }


        private void SendNextBuffer()
        {
            WrappedBuffer slice;
            if (_writeQueue.TryDequeue(out slice))
            {
                _writeBuffer.Assign(slice);
                _writeArgs.SetBuffer(slice.Buffer, slice.Offset, slice.Count);
                _socket.SendAsync(_writeArgs);
            }
            else
            {
                _writeBuffer.Reset();
            }
        }

        /// <summary>
        /// We've been disconnected (detected during a write)
        /// </summary>
        public event EventHandler<DisconnectEventArgs> Disconnected = delegate { };

        #region Nested type: WrappedBuffer

        /// <summary>
        /// Wraps a buffer so it will be disposed if disposable (and so that we can track the actual byte count in the buffer)
        /// </summary>
        private class WrappedBuffer : IBufferSlice, IDisposable
        {
            private readonly int _count;
            private readonly IBufferSlice _slice;

            public WrappedBuffer(IBufferSlice slice, int count)
            {
                _slice = slice;
                _count = count;
            }

            #region IBufferSlice Members

            public byte[] Buffer
            {
                get { return _slice.Buffer; }
            }

            public int Offset
            {
                get { return _slice.Offset; }
            }

            public int Count
            {
                get { return _count; }
            }

            #endregion

            #region IDisposable Members

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            /// <filterpriority>2</filterpriority>
            public void Dispose()
            {
                var disposable = _slice as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }

            #endregion
        }

        #endregion

        /// <summary>
        /// Reset writer.
        /// </summary>
        public void Reset()
        {
            _writeBuffer.Reset();

            WrappedBuffer slice;
            while (_writeQueue.TryDequeue(out slice))
            {
                var disposable = slice as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }

            _socket = null;
        }
    }
}