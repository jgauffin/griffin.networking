using System;
using Griffin.Networking.Buffers;
using Griffin.Networking.Messaging;
using Griffin.Networking.Pipelines.Messages;
using Griffin.Networking.Servers;

namespace Griffin.Networking.Pipelines
{
    /// <summary>
    /// Handles a pipeline for a server/client connection.
    /// </summary>
    public class PipelineServerClient : IServerService, IDownstreamHandler
    {
        private readonly IPipeline _pipeline;
        private byte[] _writeBuffer = new byte[65535]; //TODO: Use a pool.
        private IServerClientContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineServerClient" /> class.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        public PipelineServerClient(IPipeline pipeline)
        {
            _pipeline = pipeline;
            _pipeline.SetChannel(this);
        }

        /// <summary>
        /// Process message
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        /// <remarks>
        /// Should always call either <see cref="IPipelineHandlerContext.SendDownstream"/> or <see cref="IPipelineHandlerContext.SendUpstream"/>
        /// unless the handler really wants to stop the processing.
        /// </remarks>
        public virtual void HandleDownstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            var sendBuffer = message as SendBuffer;
            if (sendBuffer != null)
            {
                _context.Send(new BufferSlice(sendBuffer.Buffer, sendBuffer.Offset, sendBuffer.Count), sendBuffer.Count);
                return;
            }

            var sendSlice = message as SendSlice;
            if (sendSlice != null)
            {
                _context.Send(sendSlice.Slice, sendSlice.Length);
                return;
            }

            var send = message as SendStream;
            if (send != null)
            {
                throw new NotSupportedException();
            }

            if (message is Disconnect)
            {
                _context.Close();
                return;
            }

            throw new InvalidOperationException("Unsupported pipeline message: " + message);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
        }

        /// <summary>
        /// Assign the context which can be used to communicate with the client
        /// </summary>
        /// <param name="context">Context</param>
        public void Assign(IServerClientContext context)
        {
            _context = context;
        }

        /// <summary>
        /// A new message have been received from the remote end.
        /// </summary>
        /// <param name="message"></param>
        /// <remarks>We'll deserialize messages for you. What you receive here depends on the used <see cref="IMessageFormatterFactory"/>.</remarks>
        public void HandleReceive(object message)
        {
            var buffer = (byte[])message;
            //TODO: Fix
            //_pipeline.SendUpstream(new Received(_context.RemoteEndPoint, null, new SliceStream(readBuffer, bytesReceived)));
        }
    }
}
