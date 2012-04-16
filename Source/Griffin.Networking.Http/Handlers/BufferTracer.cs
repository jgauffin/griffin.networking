using System;
using System.Text;
using Griffin.Networking.Messages;

namespace Griffin.Networking.Http.Handlers
{
    /// <summary>
    /// Writes  content to the log
    /// </summary>
    /// <remarks>
    /// Should be the first up handler and/or the last downstream handler.
    /// </remarks>
    public class BufferTracer : IUpstreamHandler, IDownstreamHandler
    {
        private readonly ILogger _logger = LogManager.GetLogger<BufferTracer>();

        /// <summary>
        /// Handle an message
        /// </summary>
        /// <param name="context">Context unique for this handler instance</param>
        /// <param name="message">Message to process</param>
        /// <remarks>
        /// All messages that can't be handled MUST be send up the chain using <see cref="IPipelineHandlerContext.SendUpstream"/>.
        /// </remarks>
        public void HandleUpstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            var msg = message as Received;
            if (msg != null)
            {
                var str = Encoding.UTF8.GetString(msg.BufferSlice.Buffer, msg.BufferSlice.Position, msg.BufferSlice.RemainingLength);
                _logger.Trace(str);
            }

            context.SendUpstream(message);
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
        public void HandleDownstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            var msg = message as SendMessage;
            if (msg != null)
            {
                var str = Encoding.UTF8.GetString(msg.BufferSlice.Buffer, msg.BufferSlice.Position, msg.BufferSlice.RemainingLength);
               _logger.Trace(str);

            }

            var msg2 = message as SendStream;
            if (msg2 != null)
            {
                var buffer = new byte[msg2.Stream.Length];
                msg2.Stream.Read(buffer, 0, buffer.Length);
                msg2.Stream.Position = 0;
                var str = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                _logger.Trace(str);

            }

            context.SendDownstream(message);
        }
    }
}
