using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Griffin.Networking.Buffers;
using Griffin.Networking.Logging;
using Griffin.Networking.Messages;

namespace Griffin.Networking.Channels
{
    public class TcpServerChannel : IChannel
    {
        private readonly IPipelineFactory _childPipelineFactory;
        private TcpListener _listener;
        readonly BufferPool _bufferPool = new BufferPool(65535, 100, 200);
        private ILogger _logger = LogManager.GetLogger<TcpServerChannel>();

        public TcpServerChannel(IPipeline serverPipeline, IPipelineFactory childPipelineFactory, int maxNumberOfClients)
        {
            _childPipelineFactory = childPipelineFactory;
            Pipeline = serverPipeline;
            serverPipeline.SetChannel(this);
        }

        /// <summary>
        /// Gets pipeline that this channel is attached to.
        /// </summary>
        public IPipeline Pipeline { get; private set; }

        private SocketAsyncEventArgs AllocateArgs()
        {
            return new SocketAsyncEventArgs();
        }


        private void HandleException(Exception err)
        {
            SendUpstream(new PipelineFailure(err));
        }

        private void OnAcceptSocket(IAsyncResult ar)
        {
            try
            {
                Socket socket = _listener.EndAcceptSocket(ar);
                _listener.BeginAcceptSocket(OnAcceptSocket, null);
                _logger.Debug("Accepted client from " + socket.RemoteEndPoint);
                var client = new TcpServerChildChannel(_childPipelineFactory.Build(), _bufferPool);
                client.AssignSocket(socket);
                client.StartChannel();
                SendUpstream(new ChildChannelConnected(client));
            }
            catch (Exception err)
            {
                HandleException(err);
            }
        }

        private void SendUpstream(IPipelineMessage channelEvent)
        {
            Pipeline.SendUpstream(channelEvent);
        }

        /// <summary>
        /// A message have been sent through the pipeline and are ready to be handled by the channel.
        /// </summary>
        /// <param name="e">Message that the channel should process.</param>
        public void HandleDownstream(IPipelineMessage e)
        {
            if (e is BindSocket)
            {
                if (_listener != null)
                    throw new InvalidOperationException("Listener have already been specified.");

                var bind = (BindSocket)e;
                _listener = new TcpListener(bind.EndPoint);
                _listener.Start(1000);
                _listener.BeginAcceptSocket(OnAcceptSocket, null);
            }
            else if (e is Close)
            {
                _listener.Stop();
                _listener = null;
            }

        }
    }

}
