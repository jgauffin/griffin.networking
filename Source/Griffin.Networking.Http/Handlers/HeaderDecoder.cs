using System;
using Griffin.Networking.Channel;
using Griffin.Networking.Http.Implementation;
using Griffin.Networking.Http.Protocol;
using Griffin.Networking.Messages;
using Griffin.Networking.Http.Messages;

namespace Griffin.Networking.Http.Handlers
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
            if (parser == null) throw new ArgumentNullException("parser");
            _parser = parser;
        }

        /// <summary>
        /// Handle an message
        /// </summary>
        /// <param name="context">Context unique for this handler instance</param>
        /// <param name="message">Message to process</param>
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
                    if (msg.BufferSlice.RemainingLength > 0)
                        context.SendUpstream(msg);
                }

                return;
            }

            context.SendUpstream(message);
        }
    }
     
}