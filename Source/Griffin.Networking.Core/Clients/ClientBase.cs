using System;
using System.Net;
using System.Net.Sockets;
using Griffin.Networking.Buffers;
using Griffin.Networking.Messaging;

namespace Griffin.Networking.Clients
{
    /// <summary>
    /// Base class for clients.
    /// </summary>
    public abstract class ClientBase
    {
        private readonly SocketAsyncEventArgs _readArgs = new SocketAsyncEventArgs();
        private readonly BufferSlice _readBuffer = new BufferSlice(65535);
        private readonly SocketWriter _socketWriter = new SocketWriter();
        private Socket _client;
        private IPEndPoint _remoteEndPoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingClient" /> class.
        /// </summary>
        protected ClientBase()
        {
            _readArgs.Completed += OnClientRead;
            _readArgs.SetBuffer(_readBuffer.Buffer, _readBuffer.Offset, _readBuffer.Count);
        }

        /// <summary>
        /// The remote end point have disconnected (or network failure)
        /// </summary>
        /// <param name="socketAsyncEventArgs"></param>
        protected void HandleDisconnect(SocketAsyncEventArgs socketAsyncEventArgs)
        {
            Disconnected(this, new DisconnectEventArgs(socketAsyncEventArgs.SocketError));
        }

        /// <summary>
        /// We've received something from the other end
        /// </summary>
        /// <param name="buffer">Buffer containing the received bytes</param>
        /// <param name="bytesRead">Amount of bytes that we received</param>
        /// <remarks>You have to handle all bytes, anything left will be discarded.</remarks>
        protected abstract void OnReceived(IBufferSlice buffer, int bytesRead);


        private void OnClientRead(object sender, SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                OnReceived(_readBuffer, e.BytesTransferred);
                _client.ReceiveAsync(e);
            }
            else
            {
                HandleDisconnect(e);
            }
        }

        /// <summary>
        /// Send something to the remote end point
        /// </summary>
        /// <param name="slice">Slice to send. It's up to you to make sure that it's returned to the pool (if pooled)</param>
        /// <param name="count">Number of bytes in the buffer</param>
        protected void Send(IBufferSlice slice, int count)
        {
            _socketWriter.Send(slice, count);
        }

        /// <summary>
        /// Connect to an end point
        /// </summary>
        /// <param name="remoteEndPoint">end point</param>
        public void Connect(IPEndPoint remoteEndPoint)
        {
            if (remoteEndPoint == null) throw new ArgumentNullException("remoteEndPoint");
            if (_client != null)
                throw new InvalidOperationException("Client already connected.");

            _remoteEndPoint = remoteEndPoint;
            Connect();
        }

        private void Connect()
        {
            _client = new Socket(_remoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _client.Connect(_remoteEndPoint);
            _socketWriter.Assign(_client);
            var willRaiseEvent = _client.ReceiveAsync(_readArgs);
            if (!willRaiseEvent)
            {
                OnClientRead(this, _readArgs);
            }
        }

        /// <summary>
        /// Close connection and clean up
        /// </summary>
        public void Close()
        {
            if (_client == null)
                throw new InvalidOperationException("We have already been closed (or never connected).");

            _client.Shutdown(SocketShutdown.Send);
            _client.Close();
            _client.Dispose();
            _client = null;
        }

        /// <summary>
        /// Other side disconnected (or network failure)
        /// </summary>
        public virtual event EventHandler Disconnected = delegate { };
    }
}