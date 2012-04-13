using System;
using System.Net;
using Griffin.Networking.Buffers;
using Griffin.Networking.Channel;
using Griffin.Networking.Http.Implementation;
using Griffin.Networking.Http.Protocol;
using Griffin.Networking.Messages;
using Griffin.Networking.Http.Messages;

namespace Griffin.Networking.Http
{
    /// <summary>
    /// Parses the HTTP header and passes on a constructed message
    /// </summary>
    public class HeaderDecoder : IUpstreamHandler
    {
        private readonly IHttpParser _parser;
        private int _bodyBytesLeft = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderDecoder"/> class.
        /// </summary>
        /// <param name="parser">HTTP parser to use.</param>
        public HeaderDecoder(IHttpParser parser)
        {
            _parser = parser;
        }

        public void HandleUpstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            if (message is Closed)
            {
                _bodyBytesLeft = 0;
                _parser.Reset();
            }
            else if (message is Received)
            {
                var msg = (Received) message;

                // complete the body
                if (_bodyBytesLeft > 0)
                {
                    _bodyBytesLeft -= msg.BufferSlice.Count;
                    context.SendUpstream(message);
                    return;
                }

                var httpMsg = _parser.Parse(msg.BufferSlice);
                if (httpMsg != null)
                {
                    var recivedHttpMsg = new ReceivedHttpRequest((IRequest) httpMsg);
                    _bodyBytesLeft = recivedHttpMsg.HttpRequest.ContentLength;
                    _parser.Reset();

                    // send up the message to let someone else handle the body
                    context.SendUpstream(recivedHttpMsg);
                    msg.BytesHandled = msg.BufferSlice.Count;
                    context.SendUpstream(msg);
                }

                return;
            }

            context.SendUpstream(message);
        }
    }
     
}