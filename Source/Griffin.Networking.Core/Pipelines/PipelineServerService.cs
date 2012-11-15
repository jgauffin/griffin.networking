using System;
using Griffin.Networking.Buffers;
using Griffin.Networking.Messaging;
using Griffin.Networking.Pipelines.Messages;
using Griffin.Networking.Servers;

namespace Griffin.Networking.Pipelines
{
    /// <summary>
    /// Takes care of everything from a specific client in the server.
    /// </summary>
    public class PipelineServerService : IServerService, IDownstreamHandler
    {
        private readonly IPipeline _pipeline;
        private IServerClientContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineServerService" /> class.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        public PipelineServerService(IPipeline pipeline)
        {
            _pipeline = pipeline;
            _pipeline.SetChannel(this);
        }

        #region IDownstreamHandler Members

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
                _context.Send(send.Stream);
                return;
            }

            if (message is Disconnect)
            {
                _context.Close();
                return;
            }

            throw new InvalidOperationException("Unsupported pipeline message: " + message);
        }

        #endregion

        #region IServerService Members

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
        /// <remarks>
        /// We'll deserialize messages for you. What you receive here depends on the used <see cref="IMessageFormatterFactory" />.
        /// </remarks>
        public void HandleReceive(object message)
        {
            var stream = (SliceStream) message;
            _pipeline.SendUpstream(new Received(_context.RemoteEndPoint, stream));
        }

        #endregion
    }
}