using System;
using System.Net;
using System.Net.Sockets;
using Griffin.Networking.Buffers;

namespace Griffin.Networking.Servers
{
    /// <summary>
    /// Represents a client connection in the server.
    /// </summary>
    public class ServerClientContext : IServerClientContext
    {
        private readonly SocketAsyncEventArgs _readArgs;
        private readonly IBufferSlice _readBuffer;
        private readonly SliceStream _readStream;
        private readonly SocketWriter _writer;
        private IServerClient _client;
        private Socket _socket;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerClientContext" /> class.
        /// </summary>
        /// <param name="readBuffer">The read buffer.</param>
        public ServerClientContext(IBufferSlice readBuffer)
        {
            if (readBuffer == null) throw new ArgumentNullException("readBuffer");
            _readBuffer = readBuffer;
            _readStream = new SliceStream(ReadBuffer);
            _readArgs = new SocketAsyncEventArgs();
            _readArgs.Completed += OnReadCompleted;
            _writer = new SocketWriter();
            _writer.Disconnected += OnWriterDisconnect;
        }

        /// <summary>
        /// Our read buffer.
        /// </summary>
        protected IBufferSlice ReadBuffer
        {
            get { return _readBuffer; }
        }

        /// <summary>
        /// Gets remote end point
        /// </summary>
        public IPEndPoint RemoteEndPoint {get { return (IPEndPoint) _socket.RemoteEndPoint; }}

        #region IServerClientContext Members

        /// <summary>
        /// Send information to the remote end point
        /// </summary>
        /// <param name="slice">Buffer slice</param>
        /// <param name="length">Number of bytes in the slice.</param>
        public void Send(IBufferSlice slice, int length)
        {
            if (slice == null) throw new ArgumentNullException("slice");
            _writer.Send(slice, length);
        }

        /// <summary>
        /// Close connection.
        /// </summary>
        public void Close()
        {
            try
            {
                _socket.Shutdown(SocketShutdown.Send);
            }
            catch (Exception)
            {
            }

            TriggerDisconnect(SocketError.Success);
        }

        private void Cleanup()
        {
            if (_socket == null)
                return;

            _socket.Close();
            _socket = null;

            _client.Dispose();
            _client = null;

            _writer.Reset();
        }

        #endregion

        private void OnWriterDisconnect(object sender, DisconnectEventArgs e)
        {
            TriggerDisconnect(e.SocketError);
        }

        private void OnClientDisconnected(object sender, SocketAsyncEventArgs socketAsyncEventArgs)
        {
            TriggerDisconnect(socketAsyncEventArgs.SocketError);
        }

        private void TriggerDisconnect(SocketError error)
        {
            OnDisconnect(error);
            Disconnected(this, new DisconnectEventArgs(error));
            Cleanup();
        }

        /// <summary>
        /// Invoked when we've been disconnected
        /// </summary>
        /// <param name="error"><see cref="SocketError.Success"/> means that we disconnected, all other codes indicates network failure or that the remote end point disconnected</param>
        protected virtual void OnDisconnect(SocketError error)
        {
        }


        /// <summary>
        /// We've received information from the client
        /// </summary>
        /// <param name="data">The type of data depends on the server implementation.</param>
        protected virtual void TriggerClientReceive(object data)
        {
            if (data == null) throw new ArgumentNullException("data");
            _client.HandleReceive(data);
        }

        /// <summary>
        /// Remote side have disconnected (or network failure)
        /// </summary>
        /// <remarks><para>The source will be the context.</para><para>Will also be triggered when <see cref="Close"/> is invoked, but with the error code <see cref="SocketError.Success"/>.</para></remarks>
        public event EventHandler<DisconnectEventArgs> Disconnected = delegate { };

        private void OnReadCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                _readStream.Position = 0;
                _readStream.SetLength(e.BytesTransferred);
                HandleRead(_readBuffer, e.BytesTransferred);
            }
            else
            {
                OnClientDisconnected(sender, e);
            }
        }

        /// <summary>
        /// Handle incoming bytes
        /// </summary>
        /// <param name="readBuffer">Buffer containing received bytes</param>
        /// <param name="bytesReceived">Number of bytes that was recieved (will always be set, any errors have triggered <see cref="OnDisconnect"/> instead).</param>
        /// <remarks>
        /// <para>The default implementation will trigger the client with a <see cref="IBufferReader"/> as message. That means that
        /// you should not call the base method from your code.</para>
        /// </remarks>
        protected virtual void HandleRead(IBufferSlice readBuffer, int bytesReceived)
        {
            _client.HandleReceive(_readStream);
        }

        /// <summary>
        /// Assign a new socket & client to this context.
        /// </summary>
        /// <param name="socket">Socket that connected</param>
        /// <param name="client">Your own class dealing with this particular client.</param>
        public void Assign(Socket socket, IServerClient client)
        {
            if (socket == null) throw new ArgumentNullException("socket");
            if (client == null) throw new ArgumentNullException("client");
            _socket = socket;
            _client = client;
            _client.Assign(this);
            _writer.Assign(socket);

            var willRaiseEvent = _socket.ReceiveAsync(_readArgs);
            if (!willRaiseEvent)
                OnReadCompleted(_socket, _readArgs);
        }
    }
}