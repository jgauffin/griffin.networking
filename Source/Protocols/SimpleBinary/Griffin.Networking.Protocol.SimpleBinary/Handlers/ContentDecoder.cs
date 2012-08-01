using System;
using Griffin.Networking.Buffers;
using Griffin.Networking.Messages;
using Griffin.Networking.SimpleBinary.Messages;
using Griffin.Networking.SimpleBinary.Services;

namespace Griffin.Networking.SimpleBinary.Handlers
{
    /// <summary>
    /// Decodes all packets which have been mapped in <see cref="ContentMapper"/>
    /// </summary>
    /// <remarks>All packets which have not been mapped in <see cref="ContentMapper"/> will be sent upstream.</remarks>
    public class ContentDecoder : IUpstreamHandler
    {
        private readonly ContentMapper _mapper;
        private readonly BufferPool _bufferPool;
        private readonly IContentDecoder _decoder;
        private SimpleHeader _header;
        private int _bytesLeft = 0;
        private Type _packetType;
        private BufferPoolStream _stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentDecoder"/> class.
        /// </summary>
        /// <param name="mapper">Used to determine which packets to deserialize.</param>
        /// <param name="bufferPool">Used to store packet bytes before deserialization.</param>
        /// <param name="decoder">Used to deserialize the backet bytes..</param>
        public ContentDecoder(ContentMapper mapper, BufferPool bufferPool, IContentDecoder decoder)
        {
            if (mapper == null) throw new ArgumentNullException("mapper");
            if (bufferPool == null) throw new ArgumentNullException("bufferPool");
            if (decoder == null) throw new ArgumentNullException("decoder");
            _mapper = mapper;
            _bufferPool = bufferPool;
            _decoder = decoder;
        }

        /// <summary>
        /// Handle an message
        /// </summary>
        /// <param name="context">Context unique for this handler instance</param><param name="message">Message to process</param>
        /// <remarks>
        /// All messages that can't be handled MUST be send up the chain using <see cref="M:Griffin.Networking.IPipelineHandlerContext.SendUpstream(Griffin.Networking.IPipelineMessage)"/>.
        /// </remarks>
        public void HandleUpstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            var headerMsg = message as ReceivedHeader;
            if (headerMsg != null)
            {
                HandleHeader(context, message, headerMsg);
                return;
            }

            if (_packetType == null)
            {
                context.SendUpstream(message);
                return;
            }

            var msg = message as Received;
            if (msg == null)
            {
                context.SendUpstream(message);
                return;
            }


            var bytesToRead = Math.Min(msg.BufferSlice.RemainingLength, _bytesLeft);
            _stream.Write(msg.BufferSlice.Buffer, msg.BufferSlice.Position, bytesToRead);
            msg.BufferSlice.Position += bytesToRead;

            if (_stream.Length == _header.ContentLength)
            {
                _stream.Position = 0;
                var packet = _decoder.Decode(_packetType, _stream);
                _stream.SetLength(0);
                context.SendUpstream(new ReceivedPacket(packet));
                Clear();
                return;
            }

            // There are remaining received bytes.
            context.SendUpstream(msg);
        }

        private void Clear()
        {
            _stream.Dispose();
            _stream = null;
            _packetType = null;
            _header = null;
        }

        private void HandleHeader(IPipelineHandlerContext context, IPipelineMessage message, ReceivedHeader headerMsg)
        {
            _packetType = _mapper.GetPacketType(_header.ContentId);
            if (_packetType == null)
            {
                // not supported, let the rest of the pipeline
                // handle the packet.
                context.SendUpstream(message);
            }
            else
            {
                _header = headerMsg.Header;
                var buffer = _bufferPool.PopSlice();
                if (_header.ContentLength > buffer.Capacity)
                    throw new InvalidOperationException(
                        string.Format(
                            "Buffer ({0} bytes) is less than the packet content ({1} bytes). Sorry, that's not possible in the current version.",
                            buffer.Capacity, _header.ContentLength));

                _bytesLeft = _header.ContentLength;
                _stream = new BufferPoolStream(_bufferPool, buffer);
            }
        }
    }
}
