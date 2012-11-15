using System;
using Griffin.Networking.Http.Implementation;
using Griffin.Networking.Http.Messages;
using Griffin.Networking.Http.Protocol;
using Griffin.Networking.Pipelines;
using Griffin.Networking.Pipelines.Messages;

namespace Griffin.Networking.Http.Pipeline.Handlers
{
    /// <summary>
    /// Parses the HTTP header and passes on a constructed message
    /// </summary>
    public class HeaderDecoder : IUpstreamHandler
    {
        private readonly HttpHeaderParser _headerParser;
        private int _bodyBytesLeft = -1;
        private bool _headerCompleted;
        private IMessage _message;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderDecoder"/> class.
        /// </summary>
        public HeaderDecoder()
        {
            _headerParser = new HttpHeaderParser();
            _headerParser.HeaderParsed += OnHeader;
            _headerParser.Completed += OnHeaderCompleted;
            _headerParser.RequestLineParsed += OnRequestLine;
        }

        #region IUpstreamHandler Members

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
                _headerParser.Reset();
            }
            else if (message is Received)
            {
                var msg = (Received) message;

                // complete the body
                if (_bodyBytesLeft > 0)
                {
                    var bytesToSend = Math.Min(_bodyBytesLeft, msg.BufferReader.RemainingLength);
                    _bodyBytesLeft -= bytesToSend;
                    context.SendUpstream(message);
                    return;
                }

                _headerParser.Parse(msg.BufferReader);
                if (_headerCompleted)
                {
                    var recivedHttpMsg = new ReceivedHttpRequest((IRequest) _message);
                    _headerParser.Reset();
                    _headerCompleted = false;

                    context.SendUpstream(recivedHttpMsg);
                    if (msg.BufferReader.RemainingLength > 0)
                        context.SendUpstream(msg);
                }

                return;
            }

            context.SendUpstream(message);
        }

        #endregion

        private void OnRequestLine(object sender, RequestLineEventArgs e)
        {
            _message = new HttpRequest(e.Verb, e.Url, e.HttpVersion);
        }

        private void OnHeaderCompleted(object sender, EventArgs e)
        {
            _bodyBytesLeft = _message.ContentLength;
            _headerCompleted = true;
        }

        private void OnHeader(object sender, HeaderEventArgs e)
        {
            _message.AddHeader(e.Name, e.Value);
        }
    }
}