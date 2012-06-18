using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Griffin.Networking.Buffers;
using Griffin.Networking.Channel;
using Griffin.Networking.Logging;
using Griffin.Networking.Messages;

namespace Griffin.Networking.Channels
{
    /// <summary>
    /// TCP networking channel
    /// </summary>
    public class TcpChannel : IChannel, IDisposable
    {
        private readonly ILogger _logger = LogManager.GetLogger<TcpChannel>();
        private readonly IPipeline _pipeline;
        private readonly BufferPool _pool;
        private readonly BufferSlice _readBuffer;
        private Socket _socket;
        private Stream _stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpChannel"/> class.
        /// </summary>
        /// <param name="pipeline">The pipeline used to send messages upstream.</param>
        public TcpChannel(IPipeline pipeline)
        {
            _pipeline = pipeline;
            Pipeline.SetChannel(this);
            _readBuffer = new BufferSlice(new byte[65535], 0, 65535, 0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpChannel"/> class.
        /// </summary>
        /// <param name="pipeline">The pipeline used to send messages upstream.</param>
        /// <param name="pool">Buffer pool.</param>
        public TcpChannel(IPipeline pipeline, BufferPool pool)
        {
            _pipeline = pipeline;
            _pool = pool;
            Pipeline.SetChannel(this);
            _readBuffer = pool.PopSlice();
            _stream = new PeekableMemoryStream(_readBuffer.Buffer, _readBuffer.StartOffset, _readBuffer.Capacity);
        }

        /// <summary>
        /// Gets our pipeline
        /// </summary>
        protected IPipeline Pipeline
        {
            get { return _pipeline; }
        }

        /// <summary>
        /// Gets logger used by the channel
        /// </summary>
        public ILogger Logger
        {
            get { return _logger; }
        }

        #region IChannel Members

        /// <summary>
        /// A message have been sent through the pipeline and are ready to be handled by the channel.
        /// </summary>
        /// <param name="message">Message that the channel should process.</param>
        public virtual void HandleDownstream(IPipelineMessage message)
        {
            Logger.Debug("Got message " + message);

            if (message is SendSlice)
                Send((SendSlice) message);
            if (message is SendStream)
            {
                SendStream((SendStream) message);
            }
            if (message is SendBuffer)
            {
                Send((SendBuffer) message);
            }
            if (message is Disconnect)
                HandleDisconnect(new SocketException((int) SocketError.Success));
            else if (message is Close)
            {
                Disconnect();
            }

            if (message is IDisposable)
            {
                ((IDisposable) message).Dispose();
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Logger.Debug("Disposing us");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        private void Send(SendBuffer message)
        {
            _stream.Write(message.Buffer, message.Offset, message.Count);
        }

        /// <summary>
        /// Sends a message in the pipeline
        /// </summary>
        /// <param name="message">The message.</param>
        protected void SendUpstream(IPipelineMessage message)
        {
            Pipeline.SendUpstream(message);
            if (message is IDisposable)
            {
                ((IDisposable) message).Dispose();
            }
        }


        /// <summary>
        /// Disconnect the channel
        /// </summary>
        protected virtual void Disconnect()
        {
            _logger.Debug("Disconnecting socket.");
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Disconnect(true);
            Dispose(true);
            SendUpstream(new Disconnected(null));
        }

        /// <summary>
        /// Send an entire stream
        /// </summary>
        /// <param name="msg">Message</param>
        public void SendStream(SendStream msg)
        {
            msg.Stream.CopyTo(_stream);
            msg.Stream.Dispose();
        }

        /// <summary>
        /// Assign the socket which will be used by the channel
        /// </summary>
        /// <param name="socket">Socket to use</param>
        public void AssignSocket(Socket socket)
        {
            if (socket == null)
                throw new ArgumentNullException("socket");

            _stream = CreateStream(socket);
            _socket = socket;
        }

        /// <summary>
        /// Create a new networking stream
        /// </summary>
        /// <param name="socket">Socket which the stream will wrap</param>
        /// <returns>Created stream</returns>
        public virtual Stream CreateStream(Socket socket)
        {
            return new NetworkStream(socket);
        }

        private void OnRead(IAsyncResult ar)
        {
            int bytesRead;
            try
            {
                bytesRead = _stream.EndRead(ar);
                Logger.Trace("Read " + bytesRead + " bytes.");
            }
            catch (Exception err)
            {
                Logger.Warning("Got disconnected", err);
                HandleDisconnect(err);
                Dispose();
                return;
            }

            if (HandleReadBytes(bytesRead))
                StartRead();
        }

        /// <summary>
        /// Start reading from the stream
        /// </summary>
        protected void StartRead()
        {
            try
            {
                if (!_socket.Connected)
                    return;

                //var remainingCapacity = _readBuffer.Capacity - (_readBuffer.Position - _readBuffer.StartOffset);
                Logger.Debug("Reading from " + _readBuffer.Position + " length " + _readBuffer.RemainingCapacity);
                _stream.BeginRead(_readBuffer.Buffer, _readBuffer.StartOffset, _readBuffer.RemainingCapacity, OnRead,
                                  null);
            }
            catch (Exception err)
            {
                if (!(err is SocketException))
                {
                    Logger.Warning("Failed to read.", err);
                }
                HandleDisconnect(err);
                Dispose();
            }
        }

        private bool HandleReadBytes(int bytesRead)
        {
            try
            {
                if (bytesRead == 0)
                {
                    HandleDisconnect(null);
                    Dispose();
                    return false;
                }

                _readBuffer.Count += bytesRead;
                var lastOffset = -1;
                while (_readBuffer.RemainingLength != 0 && lastOffset != _readBuffer.Position)
                {
                    lastOffset = _readBuffer.Position;
                    var msg = new Received(_socket.RemoteEndPoint, _stream, _readBuffer);
                    SendUpstream(msg);
                }

                _logger.Debug(string.Format("Compacting since we got {1} bytes left from pos {0}.", _readBuffer.Position,
                                            _readBuffer.RemainingLength));
                _readBuffer.Compact();
                return true;
            }
            catch (Exception err)
            {
                Logger.Warning("A pipeline handler threw an exception.", err);
                Pipeline.SendUpstream(new PipelineFailure(err));

                Dispose();
                return false;
            }
        }


        private void HandleDisconnect(Exception err)
        {
            Logger.Warning("Got disconnected");
            Pipeline.SendUpstream(new Disconnected(err));
            Dispose(true);
        }

        /// <summary>
        /// Send a buffer slice.
        /// </summary>
        /// <param name="message"></param>
        public virtual void Send(SendSlice message)
        {
            if (_socket == null)
                throw new InvalidOperationException("Socket is disconnected");


            var buffer = message.BufferSlice;
            try
            {
                var tmp = Encoding.UTF8.GetString(buffer.Buffer, buffer.StartOffset, buffer.Count);
                Logger.Trace(tmp);
                Logger.Debug("Sending " + buffer.Count + " bytes");
                _stream.BeginWrite(buffer.Buffer, buffer.StartOffset, buffer.Count, OnSent, message.BufferSlice);
            }
            catch (Exception err)
            {
                HandleDisconnect(err);
                Dispose();
            }
        }

        private void OnSent(IAsyncResult ar)
        {
            try
            {
                _stream.EndWrite(ar);
                Logger.Trace("Send completed");
                Pipeline.SendUpstream(new Sent((BufferSlice) ar.AsyncState));
            }
            catch (Exception err)
            {
                HandleDisconnect(err);
                Dispose();
            }
        }


        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        public virtual void Dispose(bool disposing)
        {
            if (!disposing || _socket == null)
                return;

            Pipeline.SendUpstream(new Closed());

            try
            {
                if (_socket.Connected)
                {
                    _socket.Shutdown(SocketShutdown.Both);
                    _socket.Disconnect(true);
                }
                _socket.Dispose();
                _stream.Close();
                _stream.Dispose();
            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
            }


            _socket = null;

            if (_pool != null)
                _pool.Push(_readBuffer.Buffer);
            else if (_readBuffer is IDisposable)
                ((IDisposable) _readBuffer).Dispose();
        }
    }
}