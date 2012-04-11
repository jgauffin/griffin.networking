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
    /// A HTTP parser using delegates to switch parsing methods.
    /// </summary>
    public class Decoder : IUpstreamHandler
    {
        private readonly IHttpParser _parser;

        /// <summary>
        /// Initializes a new instance of the <see cref="Decoder"/> class.
        /// </summary>
        public Decoder(IHttpParser parser)
        {
            _parser = parser;
        }

        public void HandleUpstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            if (message is Closed)
            {
                _parser.Reset();
            }
            else if (message is Received)
            {
                var msg = (Received) message;
                var httpMsg = _parser.Parse(msg.BufferSlice);

                if (httpMsg != null)
                {
                    var recivedHttpMsg = new ReceivedHttpRequest((IRequest) httpMsg);
                    _parser.Reset();
                    context.SendUpstream(recivedHttpMsg);
                }

                return;
            }

            context.SendUpstream(message);
        }
    }
     
}