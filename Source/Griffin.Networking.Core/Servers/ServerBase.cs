using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Griffin.Networking.Buffers;

namespace Griffin.Networking.Servers
{
    /// <summary>
    /// Base class for servers.
    /// </summary>
    /// <remarks>Contains most of the logic, but do not dictate how you should handle clients.</remarks>
    public abstract class ServerBase
    {
        private readonly BufferSliceStack _bufferSliceStack;
        private readonly ConcurrentStack<ServerClientContext> _contexts = new ConcurrentStack<ServerClientContext>();
        private readonly int _maxAmountOfConnection;
        private readonly Semaphore _maxNumberAcceptedClients;
        private Socket _listener;
        private int _numConnectedSockets;

        /// <summary>
        /// Initializes a new instance of the <see cref="Server" /> class.
        /// </summary>
        protected ServerBase(ServerConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            configuration.Validate();

            _numConnectedSockets = 0;
            _maxAmountOfConnection = configuration.MaximumNumberOfClients;
            _maxNumberAcceptedClients = new Semaphore(configuration.MaximumNumberOfClients,
                                                      configuration.MaximumNumberOfClients);

            // *2 since we need one for each send/receive pair.
            _bufferSliceStack = new BufferSliceStack(configuration.MaximumNumberOfClients*2, configuration.BufferSize);
        }


        private void Init()
        {
            for (var i = 0; i < _maxAmountOfConnection; i++)
            {
                var context = CreateClientContext(_bufferSliceStack.Pop());
                context.Disconnected += OnClientDisconnectedInternal;
                context.UnhandledExceptionCaught += OnClientException;
                context.SetWriteBuffer(_bufferSliceStack.Pop());
                _contexts.Push(context);
            }
        }

        private void OnClientException(object sender, ClientExceptionEventArgs e)
        {
            UnhandledClientExceptionCaught(this, e);
        }

        /// <summary>
        /// Create a new client context
        /// </summary>
        /// <param name="readBuffer">The read buffer to use</param>
        /// <returns>Created context</returns>
        /// <remarks>The contexts are created at startup and then reused during the server lifetime. A context
        /// is assigned to a socket each time we've accepted one using the listener.</remarks>
        protected virtual ServerClientContext CreateClientContext(IBufferSlice readBuffer)
        {
            return new ServerClientContext(readBuffer);
        }

        /// <summary>
        /// A client has disconnected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClientDisconnectedInternal(object sender, EventArgs e)
        {
            var context = (ServerClientContext) sender;
            OnClientDisconnected(context);
            Interlocked.Decrement(ref _numConnectedSockets);
            _maxNumberAcceptedClients.Release();
            context.Reset();
            _contexts.Push(context);
        }

        /// <summary>
        /// A client has disconnected from the server (either network failure or by the remote end point)
        /// </summary>
        /// <param name="context">Disconnected client</param>
        /// <remarks>Calls to <see cref="ServerClientContext.Close()"/> will also trigger this method, but with <see cref="SocketError.Success"/>. 
        /// <para>The method is typically used to clean up your own implementation. The context, socket ETC have already been cleaned up.</para></remarks>
        protected virtual void OnClientDisconnected(ServerClientContext context)
        {
        }

        /// <summary>
        /// Start server and begin accepting end points.
        /// </summary>
        /// <param name="localEndPoint">End point that the server should listen on.</param>
        public void Start(IPEndPoint localEndPoint)
        {
            if (_listener != null)
                throw new InvalidOperationException("Server already started.");

            Init();
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listener.Bind(localEndPoint);
            _listener.Listen(100);

            var listenerArgs = new SocketAsyncEventArgs();
            listenerArgs.Completed += OnAccept;
            StartAccept(listenerArgs);
        }

        /// <summary>
        /// Gets port that the server is listening on
        /// </summary>
        /// <remarks>Useful if you specify <c>0</c> as port in <see cref="Start"/> (which means that the OS should pick a free port)</remarks>
        public int LocalPort
        {
            get { return ((IPEndPoint) _listener.LocalEndPoint).Port; }
        }

        private void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            _maxNumberAcceptedClients.WaitOne();
            acceptEventArg.AcceptSocket = null;
            var willRaiseEvent = _listener.AcceptAsync(acceptEventArg);
            if (!willRaiseEvent)
            {
                OnAccept(this, acceptEventArg);
            }
        }


        private void OnAccept(object sender, SocketAsyncEventArgs e)
        {
            Interlocked.Increment(ref _numConnectedSockets);

            ServerClientContext context;
            if (!_contexts.TryPop(out context))
                throw new InvalidOperationException("Failed to get a new client context, all is currently in use.");

            if (!ValidateClient(e.AcceptSocket))
            {
                try
                {
                    e.AcceptSocket.Shutdown(SocketShutdown.Send);
                }
                catch
                {
                }
                e.AcceptSocket.Close();
                return;
            }

            var client = CreateClient(e.AcceptSocket.RemoteEndPoint);
            context.Assign(e.AcceptSocket, client);
            OnClientConnected(context);

            StartAccept(e);
        }

        /// <summary>
        /// Create a new object which will handle all communication to/from a specific client.
        /// </summary>
        /// <param name="remoteEndPoint">Remote end point</param>
        /// <returns>Created client</returns>
        protected abstract INetworkService CreateClient(EndPoint remoteEndPoint);

        /// <summary>
        /// A new client have connected
        /// </summary>
        /// <param name="context">Client context</param>
        /// <remarks>Invoked when a client has been validated and successfully been added.</remarks>
        /// <seealso cref="ValidateClient"/>
        protected virtual void OnClientConnected(ServerClientContext context)
        {
        }

        /// <summary>
        /// A new client have connected
        /// </summary>
        /// <param name="acceptedSocket">Socket for the client</param>
        /// <returns><c>true</c> if the client can be accepted; <c>false</c> to disconnect the client.</returns>
        /// <remarks>Use this method to filter out any unwanted clients. Feel free to use it for any handshake etc.</remarks>
        /// <seealso cref="OnClientConnected"/>
        protected virtual bool ValidateClient(Socket acceptedSocket)
        {
            return true;
        }

        /// <summary>
        /// Stop accepting new connections
        /// </summary>
        /// <remarks>Any existing connections will continue to run until they disconnect.</remarks>
        public void Stop()
        {
            _listener.Close();
            _listener = null;
        }

        /// <summary>
        /// An unhandled exception has been caught for one of the clients.
        /// </summary>
        /// <remarks>Use the <see cref="ClientExceptionEventArgs.CanContinue"/> to flag if processing should be aborted or not.</remarks>
        public event EventHandler<ClientExceptionEventArgs> UnhandledClientExceptionCaught = delegate { };
    }
}