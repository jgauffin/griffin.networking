using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Networking.Pipelines.Messages;
using Griffin.Networking.SimpleBinary.Messages;

namespace Griffin.Networking.SimpleBinary.Handlers
{
    /// <summary>
    /// Decodes incoming header.
    /// </summary>
    public class HeaderDecoder : IUpstreamHandler
    {
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
            if (msg == null)
            {
                context.SendUpstream(message);
                return;
            }

            // byte + byte + int
            if (msg.BufferSlice.RemainingLength < 6)
            {
                return;
            }

            var header = new SimpleHeader
            {
                Version = msg.BufferSlice.Buffer[msg.BufferSlice.Position++],
                ContentId = msg.BufferSlice.Buffer[msg.BufferSlice.Position++],
                ContentLength = BitConverter.ToInt32(msg.BufferSlice.Buffer, msg.BufferSlice.Position)
            };
            msg.BufferSlice.Position += 4;
            context.SendUpstream(new ReceivedHeader(header));

            if (msg.BufferSlice.RemainingLength > 0)
                context.SendUpstream(msg);            
        }
    }
}
