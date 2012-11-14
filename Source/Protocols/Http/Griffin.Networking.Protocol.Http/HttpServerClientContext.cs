using System;
using System.IO;
using Griffin.Networking.Buffers;
using Griffin.Networking.Http.Implementation;
using Griffin.Networking.Http.Pipeline.Handlers;
using Griffin.Networking.Http.Protocol;
using Griffin.Networking.Servers;

namespace Griffin.Networking.Http
{
    /// <summary>
    /// Lightweight client which just parses the HTTP message and sends it along.
    /// </summary>
    public class HttpServerClientContext : ServerClientContext
    {
        SliceStream _receiveStream;
        private static IBufferSliceStack _stack = new BufferSliceStack(100, 65535);
        HttpHeaderParser _headerParser = new HttpHeaderParser();
        private IMessage _message;
        private Stream _bodyStream;
        private IBufferSlice _bodySlice;
        private int _bodyBytestLeft = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpServerClient" /> class.
        /// </summary>
        /// <param name="readBuffer">The read buffer.</param>
        public HttpServerClientContext(IBufferSlice readBuffer)
            : base(readBuffer)
        {
            _receiveStream = new SliceStream(readBuffer);
            _headerParser.HeaderParsed += OnHeader;
            _headerParser.Completed += OnHeaderComplete;
            _headerParser.RequestLineParsed += OnRequestLine;
            _bodySlice = _stack.Pop();
        }

        private void OnRequestLine(object sender, RequestLineEventArgs e)
        {
            _message = new HttpRequest(e.Verb, e.Url, e.HttpVersion);
        }

        private void OnHeaderComplete(object sender, EventArgs e)
        {
            _bodyBytestLeft = _message.ContentLength;
            if (_message.ContentLength > _bodySlice.Count)
                _bodyStream = new FileStream(Path.GetTempFileName(), FileMode.Create);
            else
                _bodyStream = new SliceStream(_bodySlice);
        }

        private void OnHeader(object sender, HeaderEventArgs e)
        {
            _message.AddHeader(e.Name, e.Value);
        }

        /// <summary>
        /// Handle incoming bytes
        /// </summary>
        /// <param name="readBuffer">Buffer containing received bytes</param>
        /// <param name="bytesReceived">Number of bytes that was recieved (will always be set, any errors have triggered <see cref="ServerClientContext.OnDisconnect" /> instead).</param>
        /// <remarks>
        /// The default implementation will trigger the client with a <see cref="IBufferReader" /> as message. That means that
        /// you should not call the base method from your code.
        /// </remarks>
        protected override void HandleRead(IBufferSlice readBuffer, int bytesReceived)
        {
            _receiveStream.SetLength(bytesReceived);

            _headerParser.Parse(_receiveStream);
            if (_bodyBytestLeft > 0)
            {
                var bytesToRead = Math.Min(_receiveStream.RemainingLength, _bodyBytestLeft);
                _receiveStream.CopyTo(_bodyStream, bytesToRead);

                if (_receiveStream.RemainingLength > 0)
                {
                    _headerParser.Parse(_receiveStream);
                }
            }
        }

        protected override void OnDisconnect(System.Net.Sockets.SocketError error)
        {
            
        }

        public virtual void Send(IMessage message)
        {
            var slice = _stack.Pop();
            var stream = new SliceStream(slice);
            var serializer = new HttpHeaderSerializer();
            serializer.SerializeResponse((IResponse)message, stream);
            Send(slice, (int) stream.Length);

            slice = _stack.Pop();

        }
    }
}