using System;
using System.IO;
using Griffin.Networking.Buffers;
using Griffin.Networking.JsonRpc.Messages;
using Griffin.Networking.Messages;
using Newtonsoft.Json;

namespace Griffin.Networking.JsonRpc.Handlers
{
    /// <summary>
    /// Deserializes all body bytes (<see cref="Received"/> message) into a JSON <see cref="Request"/> object which is sent upstream with <see cref="ReceivedRequest"/>.
    /// </summary>
    public class BodyDecoder : IUpstreamHandler
    {
        private static readonly BufferPool _bufferPool = new BufferPool(65535, 50, 50);
        private readonly BufferPoolStream _stream;
        private SimpleHeader _header;

        /// <summary>
        /// Initializes a new instance of the <see cref="BodyDecoder"/> class.
        /// </summary>
        public BodyDecoder()
        {
            var slice = _bufferPool.PopSlice();
            _stream = new BufferPoolStream(_bufferPool, slice);
        }

        #region IUpstreamHandler Members

        /// <summary>
        /// Handle an message
        /// </summary>
        /// <param name="context">Context unique for this handler instance</param>
        /// <param name="message">Message to process</param>
        public void HandleUpstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            var headerMsg = message as ReceivedHeader;
            if (headerMsg != null)
            {
                _header = headerMsg.Header;
                if (_header.Length > 65535)
                {
                    var error = new ErrorResponse("-9999", new RpcError
                                                               {
                                                                   Code = RpcErrorCode.InvalidRequest,
                                                                   Message =
                                                                       "Support requests which is at most 655355 bytes.",
                                                               });
                    context.SendDownstream(new SendResponse(error));
                }

                return;
            }

            var received = message as Received;
            if (received != null)
            {
                var count = Math.Min(received.BufferSlice.RemainingLength, _header.Length);
                _stream.Write(received.BufferSlice.Buffer, received.BufferSlice.Position, count);
                received.BufferSlice.Position += count;

                if (_stream.Length == _header.Length)
                {
                    _stream.Position = 0;
                    var request = DeserializeRequest(_stream);
                    context.SendUpstream(new ReceivedRequest(request));
                }

                return;
            }

            context.SendUpstream(message);
        }

        #endregion

        /// <summary>
        /// Deserialize the stream contents into a JSON object
        /// </summary>
        /// <param name="body">Stream with JSON</param>
        /// <returns>Generated request.</returns>
        protected virtual Request DeserializeRequest(BufferPoolStream body)
        {
            var reader = new StreamReader(body);
            var json = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<Request>(json);
        }
    }
}