using System;
using System.Text;
using System.Xml;
using Griffin.Networking.Buffers;
using Griffin.Networking.Messages;
using Griffin.Networking.SimpleBinary.Messages;
using Griffin.Networking.SimpleBinary.Services;

namespace Griffin.Networking.SimpleBinary.Handlers
{
    /// <summary>
    /// Encodes <see cref="SendResponse"/> content and sends away the data.
    /// </summary>
    public class ResponseEncoder : IDownstreamHandler
    {
        private readonly ContentMapper _mapper;
        private readonly IContentEncoder _encoder;
        private static readonly BufferPool _bufferPool = new BufferPool(65535, 50, 100);

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseEncoder"/> class.
        /// </summary>
        /// <param name="mapper">Used to find content id for the send objects.</param>
        /// <param name="encoder">Serializes the sent object.</param>
        public ResponseEncoder(ContentMapper mapper, IContentEncoder encoder)
        {
            if (mapper == null) throw new ArgumentNullException("mapper");
            if (encoder == null) throw new ArgumentNullException("encoder");
            _mapper = mapper;
            _encoder = encoder;
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
            var msg =  message as SendResponse;
            if (msg == null)
            {
                context.SendDownstream(message);
                return;
            }

            var buffer = _bufferPool.PopSlice();
            var stream = new BufferPoolStream(_bufferPool, buffer);
            _encoder.Encode(msg.Response, stream);
            stream.Position = 0;

            // send header
            var header = new byte[6];
            header[0] = 1;
            header[1] = _mapper.GetContentId(msg.Response);
            var lengthBuffer = BitConverter.GetBytes(buffer.Count);
            Buffer.BlockCopy(lengthBuffer, 0, header, 2, lengthBuffer.Length);
            context.SendDownstream(new SendBuffer(header, 0, 6));

            // send body
            context.SendDownstream(new SendStream(stream));
        }
    }

}
