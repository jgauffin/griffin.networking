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
        private readonly ConcurrentQueue<ISocketWriterJob> _writeQueue = new ConcurrentQueue<ISocketWriterJob>();
        private ISocketWriterJob _currentJob;
        private Socket _socket;


        /// <summary>
        /// Initializes a new instance of the <see cref="SocketWriter" /> class.
        /// </summary>
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
        /// <exception cref="System.ArgumentNullException">slice</exception>
        /// <exception cref="System.InvalidOperationException">Socket as not been Assign():ed.</exception>
        /// <seealso cref="StreamSocketWriterJob"/>
        /// <seealso cref="SliceSocketWriterJob"/>
        public void Send(ISocketWriterJob job)
        {
            if (job == null) throw new ArgumentNullException("job");
            if (_socket == null)
                throw new InvalidOperationException("Socket as not been Assign():ed.");

            if (_currentJob != null)
            {
                _writeQueue.Enqueue(job);
                return;
            }

            _currentJob = job;
            Console.WriteLine("Sending " + _currentJob);
            _currentJob.Write(_writeArgs);

            bool isPending = _socket.SendAsync(_writeArgs);
            if (!isPending)
                HandleWriteCompleted(_writeArgs.SocketError, _writeArgs.BytesTransferred);
        }

        private void HandleWriteCompleted(SocketError error, int bytesTransferred)
        {
            if (error == SocketError.Success)
            {
                if (bytesTransferred == 0)
                    throw new InvalidOperationException("Failed to send bytes? " + bytesTransferred);
                if (_currentJob.WriteCompleted(bytesTransferred))
                {
                    _currentJob.Dispose();
                    if (!_writeQueue.TryDequeue(out _currentJob))
                    {
                        _currentJob = null;
                        return;
                    }
                }

                Console.WriteLine("Sending " + _currentJob);
                _currentJob.Write(_writeArgs);
                bool isPending = _socket.SendAsync(_writeArgs);
                if (!isPending)
                    HandleWriteCompleted(_writeArgs.SocketError, _writeArgs.BytesTransferred);
            }
            else
            {
                if (error == SocketError.Success)
                    error = SocketError.ConnectionReset;
                HandleDisconnect(error);
            }
        }

        private void HandleDisconnect(SocketError error)
        {
            Reset();
            Disconnected(this, new DisconnectEventArgs(error));
        }

        /// <summary>
        /// We've been disconnected (detected during a write)
        /// </summary>
        public event EventHandler<DisconnectEventArgs> Disconnected = delegate { };

        /// <summary>
        /// Reset writer.
        /// </summary>
        public void Reset()
        {
            if (_currentJob != null)
                _currentJob.Dispose();
            _currentJob = null;


            ISocketWriterJob job;
            while (_writeQueue.TryDequeue(out job))
            {
                job.Dispose();
            }

            _socket = null;
        }

        /// <summary>
        /// Assign a buffer which can be used during writes.
        /// </summary>
        /// <param name="bufferSlice">Buffer</param>
        /// <remarks>The buffer is stored as <c>UserToken</c> for the AsyncEventArgs. Do not change the token, but feel free to use it for the current write.</remarks>
        public void SetBuffer(IBufferSlice bufferSlice)
        {
            if (bufferSlice == null) throw new ArgumentNullException("bufferSlice");
            _writeArgs.UserToken = bufferSlice;
        }
    }
}