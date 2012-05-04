using System;
using System.IO;
using System.Net;
using Griffin.Networking.Buffers;
using Griffin.Networking.Http.Implementation;
using Griffin.Networking.Http.Messages;
using Griffin.Networking.Http.Protocol;
using Griffin.Networking.Http.Services;
using Griffin.Networking.Messages;

namespace Griffin.Networking.Http.Handlers
{
    /// <summary>
    /// Can decode bodies.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Will not pass on the <see cref="ReceivedHttpRequest"/> message until the body have been parsed successfully.
    /// </para>
    /// <para>
    ///  The decoder uses a BufferPool buffer to host the body contents.The decoder will switch to <see cref="FileStream"/> for bodies larger than the <c>bufferSize</c> contructor parameter.
    /// This will of course hurt performance but keep the memory usage per request down.
    /// </para>
    /// </remarks>
    public class BodyDecoder : IUpstreamHandler
    {
        private readonly IBodyDecoder _decoderService;
        private readonly int _bufferSize;
        private readonly int _sizeLimit;
        private IMessage _currentMessage;
        private static BufferPool _bufferPool ;

        /// <summary>
        /// Initializes a new instance of the <see cref="BodyDecoder"/> class.
        /// </summary>
        /// <param name="decoderService">The decoder service.</param>
        /// <param name="bufferSize">Buffer size of each buffer in the pool. Read the remarks at <see cref="BodyDecoder"/></param>
        /// <param name="sizeLimit">Maximum size of the body in bytes. Larger content will generate a <see cref="HttpStatusCode.RequestEntityTooLarge"/> response which will
        /// be sent back to the client.</param>
        public BodyDecoder(IBodyDecoder decoderService, int bufferSize, int sizeLimit)
        {
            if (decoderService == null) throw new ArgumentNullException("decoderService");
            _decoderService = decoderService;
            _bufferSize = bufferSize;
            _sizeLimit = sizeLimit;
            _bufferPool = new BufferPool(_bufferSize, 10, 1000);
        }

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
            var httpmsg = message as ReceivedHttpRequest;
            if (httpmsg != null)
            {
                if (httpmsg.HttpRequest.ContentLength > _sizeLimit)
                {
                    var response = httpmsg.HttpRequest.CreateResponse(HttpStatusCode.RequestEntityTooLarge,
                                                                      string.Format("Max body size is {0} bytes.",
                                                                                    _sizeLimit));
                    context.SendDownstream(new SendHttpResponse(httpmsg.HttpRequest, response));
                    return;
                }

                if (httpmsg.HttpRequest.ContentLength == 0)
                {
                    context.SendUpstream(message);
                    return;
                }

                _currentMessage = httpmsg.HttpRequest;
                return; // don't send the request upwards.
            }

            var msg = message as Received;
            if (msg != null)
            {
                var result = ParseBody(msg.BufferSlice);
                if (!result)
                    return;

                _currentMessage.Body.Position = 0;
                _decoderService.Decode((IRequest)_currentMessage);
                context.SendUpstream(new ReceivedHttpRequest((HttpRequest)_currentMessage));
                _currentMessage = null;
                return;
            }

            // pass on all other messages
            context.SendUpstream(message);
        }

        /// <summary>
        /// Parser method to copy all body bytes.
        /// </summary>
        /// <param name="bufferSlice"> </param>
        /// <returns></returns>
        /// <remarks>Needed since a TCP packet can contain multiple messages
        /// after each other, or partial messages.</remarks>
        private bool ParseBody(BufferSlice bufferSlice)
        {
            if (bufferSlice.RemainingLength == 0)
                return false;

            if (_currentMessage.Body == null)
            {
                if (_currentMessage.ContentLength > _bufferSize)
                    _currentMessage.Body = new FileStream(Path.GetTempFileName(), FileMode.CreateNew);
                else
                {
                    var slice = _bufferPool.PopSlice();
                    _currentMessage.Body = new BufferPoolStream(_bufferPool, slice);
                }
            }

            var bytesLeft = (int)Math.Min(_currentMessage.ContentLength - _currentMessage.Body.Length, bufferSlice.RemainingLength);
            _currentMessage.Body.Write(bufferSlice.Buffer, bufferSlice.Position, bytesLeft);
            bufferSlice.Position += bytesLeft;
            return _currentMessage.Body.Length == _currentMessage.ContentLength;
        }

    }
}

