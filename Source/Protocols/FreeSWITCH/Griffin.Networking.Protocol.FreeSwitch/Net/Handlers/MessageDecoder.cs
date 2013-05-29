using System;
using System.Diagnostics;
using System.Text;
using Griffin.Networking.Logging;
using Griffin.Networking.Pipelines;
using Griffin.Networking.Pipelines.Messages;
using Griffin.Networking.Protocol.FreeSwitch.Net.Messages;

namespace Griffin.Networking.Protocol.FreeSwitch.Net.Handlers
{
    /// <summary>
    /// Decodes all messages and should typically be the first handler in the pipeline.
    /// </summary>
    public class MessageDecoder : IUpstreamHandler
    {
        private readonly ILogger _logger = LogManager.GetLogger<MessageDecoder>();
        private DecoderContext _context;

        public MessageDecoder()
        {
            _context = new DecoderContext {ParserMethod = ParseBeforeHeader};
        }

        #region IUpstreamHandler Members

        public void HandleUpstream(IPipelineHandlerContext ctx, IPipelineMessage message)
        {
            if (message is Connected)
            {
                _context = new DecoderContext {ParserMethod = ParseBeforeHeader};
            }
            else if (message is Closed)
            {
                _context = null;
            }
            if (!(message is Received))
            {
                ctx.SendUpstream(message);
                return;
            }


            var evt = (Received) message;
            var buffer = evt.BufferReader;
            _logger.Trace("Pos: " + buffer.Position);
            _context.Reader.Assign(buffer);


            try
            {
                var canrun = true;
                while (canrun)
                {
                    _logger.Trace("Parsing using " + _context.ParserMethod.Method.Name);
                    canrun = _context.ParserMethod();
                }
            }
            catch (Exception err)
            {
                _logger.Error("Failed to parse message.", err);
                ctx.SendUpstream(new PipelineFailure(err));
                return;
            }

            if (!_context.IsComplete)
            {
                //_leftOvers = Encoding.UTF8.GetString(buffer.Buffer, buffer.Offset, buffer.RemainingLength);
                return; // Need more data from the channel
            }

            _context.ParserMethod = ParseBeforeHeader;
            _logger.Debug("Got message " + _context.Message.Headers["Content-Type"]);
            if (string.IsNullOrEmpty(_context.Message.Headers["Content-Type"]))
            {
                _logger.Warning("Malformed message\r\n" +
                                Encoding.ASCII.GetString(buffer.Buffer, buffer.StartOffset, buffer.Count));
                Debugger.Break();
            }

            ctx.SendUpstream(new ReceivedMessage(_context.Message));
            _context.Message = new Message();
        }

        #endregion

        private bool ParseBeforeHeader()
        {
            while (_context.Reader.Peek == '\n')
                _context.Reader.Consume();

            _context.ParserMethod = ParseHeaderName;
            return true;
        }

        private bool ParseBody()
        {
            if (_context.Message.ContentLength <= 0)
            {
                _logger.Trace("No body");
                _context.IsComplete = true;
                return false;
            }

            var bytesLeft = _context.Message.ContentLength - (int) _context.Message.Body.Length;
            var count = Math.Min(bytesLeft, _context.Reader.RemainingLength);
            _context.Message.Append(_context.Reader.Buffer, _context.Reader.Index, count);
            _context.IsComplete = _context.Message.Body.Length == _context.Message.ContentLength;
            _context.Reader.Index += count;
            if (_context.IsComplete)
            {
                _logger.Trace("Body complete");
                _context.Message.Body.Position = 0;
            }
            else
                _logger.Debug(string.Format("{0} bytes left.",
                                            _context.Message.ContentLength - _context.Message.Body.Length));
            return false;
        }

        private bool ParseHeaderName()
        {
            // empty line == go for body.
            if (_context.Reader.Current == '\n')
            {
                _context.Reader.Consume();
                _context.ParserMethod = ParseBody;
                return true;
            }

            _context.Reader.ConsumeWhiteSpaces();
            _context.CurrentHeaderName = _context.Reader.ReadUntil(": ");
            _logger.Trace("Header name: " + _context.CurrentHeaderName);
            if (_context.CurrentHeaderName == null)
                return false;

            if (_context.Reader.Current != ':')
            {
                _context.Reader.ConsumeWhiteSpaces();
                _context.Reader.ReadUntil(':');
            }
            _context.Reader.Consume(':');

            _context.ParserMethod = ParseHeaderValue;
            return true;
        }

        private bool ParseHeaderValue()
        {
            _context.Reader.ConsumeWhiteSpaces();
            var value = _context.Reader.ReadLine();
            _logger.Trace("Header value: " + value);
            if (value == null)
                return false;

            _context.Message.Headers.Add(_context.CurrentHeaderName, value);
            _context.CurrentHeaderName = null;

            _context.ParserMethod = ParseHeaderName;
            return true;
        }

        public void Reset()
        {
            _context.Message.Reset();
            _context.ParserMethod = ParseBeforeHeader;
            _context.IsComplete = false;
            _context.CurrentHeaderName = null;
        }

        #region Nested type: DecoderContext

        private class DecoderContext
        {
            public DecoderContext()
            {
                Message = new Message();
                CurrentHeaderName = "";
                IsComplete = false;
                Reader = new BufferSliceReader();
            }

            public Encoding Encoding { get; set; }

            public Message Message { get; set; }
            public BufferSliceReader Reader { get; set; }
            public string CurrentHeaderName { get; set; }
            public bool IsComplete { get; set; }
            public ParserMethod ParserMethod { get; set; }
        }

        #endregion

        #region Nested type: ParserMethod

        private delegate bool ParserMethod();

        #endregion
    }
}