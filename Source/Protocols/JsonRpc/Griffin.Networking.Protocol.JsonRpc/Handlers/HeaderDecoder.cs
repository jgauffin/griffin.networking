using System;
using Griffin.Networking.JsonRpc.Messages;
using Griffin.Networking.Pipelines;
using Griffin.Networking.Pipelines.Messages;

namespace Griffin.Networking.JsonRpc.Handlers
{
    /// <summary>
    /// Handles <see cref="Received"/> and decodes the <see cref="SimpleHeader"/> which is sent in the message <see cref="ReceivedHeader"/>.
    /// </summary>
    public class HeaderDecoder : IUpstreamHandler
    {
        private readonly byte[] _header = new byte[5];
        private int _bytesLeft = 5;
        private int _position;

        #region IUpstreamHandler Members

        /// <summary>
        /// Handle an message
        /// </summary>
        /// <param name="context">Context unique for this handler instance</param>
        /// <param name="message">Message to process</param>
        public void HandleUpstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            var msg = message as Received;
            if (msg == null)
            {
                context.SendUpstream(message);
                return;
            }

            var bytesToCopy = Math.Min(msg.BufferReader.Count, _bytesLeft);
            msg.BufferReader.Read(_header, _position, bytesToCopy);
            _position += bytesToCopy;
            _bytesLeft -= bytesToCopy;

            if (_bytesLeft > 0)
            {
                return;
            }

            var header = new SimpleHeader
                {
                    Version = _header[0],
                    Length = BitConverter.ToInt32(_header, 1)
                };

            _bytesLeft = 5;
            _position = 0;
            context.SendUpstream(new ReceivedHeader(header));

            if (msg.BufferReader.Position < msg.BufferReader.Count)
                context.SendUpstream(msg);
        }

        #endregion
    }
}