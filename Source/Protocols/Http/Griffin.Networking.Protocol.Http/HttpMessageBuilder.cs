using System;
using System.Collections.Concurrent;
using System.IO;
using Griffin.Networking.Buffers;
using Griffin.Networking.Http.Implementation;
using Griffin.Networking.Http.Protocol;
using Griffin.Networking.Messaging;

namespace Griffin.Networking.Http
{
    /// <summary>
    /// Builds HTTP messags from incoming bytes.
    /// </summary>
    public class HttpMessageBuilder : IMessageBuilder, IDisposable
    {
        private static readonly IBufferSliceStack _stack = new BufferSliceStack(100, 65535);
        private readonly IBufferSlice _bodySlice;
        private readonly HttpHeaderParser _headerParser = new HttpHeaderParser();
        private readonly ConcurrentQueue<IMessage> _messages = new ConcurrentQueue<IMessage>();
        private int _bodyBytestLeft;
        private Stream _bodyStream;
        private IMessage _message;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMessageBuilder" /> class.
        /// </summary>
        public HttpMessageBuilder()
        {
            _headerParser.HeaderParsed += OnHeader;
            _headerParser.Completed += OnHeaderComplete;
            _headerParser.RequestLineParsed += OnRequestLine;
            _bodySlice = _stack.Pop();
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            _stack.Push(_bodySlice);
        }

        #endregion

        #region IMessageBuilder Members

        /// <summary>
        /// Append more bytes to your message building
        /// </summary>
        /// <param name="reader">Contains bytes which was received from the other end</param>
        /// <returns><c>true</c> if a complete message has been built; otherwise <c>false</c>.</returns>
        /// <remarks>You must handle/read everything which is available in the buffer</remarks>
        public bool Append(IBufferReader reader)
        {
            _headerParser.Parse(reader);
            if (_bodyBytestLeft > 0)
            {
                var bytesToRead = Math.Min(reader.RemainingLength, _bodyBytestLeft);
                reader.CopyTo(_bodyStream, bytesToRead);
                _bodyBytestLeft -= bytesToRead;

                if (_bodyBytestLeft == 0)
                {
                    _messages.Enqueue(_message);
                    _message = null;
                }

                if (reader.RemainingLength > 0)
                {
                    _headerParser.Parse(reader);
                }
            }

            return _messages.Count > 0;
        }

        /// <summary>
        /// Try to dequeue a message
        /// </summary>
        /// <param name="message">Message that the builder has built.</param>
        /// <returns><c>true</c> if a message was available; otherwise <c>false</c>.</returns>
        public bool TryDequeue(out object message)
        {
            IMessage msg;
            var result = _messages.TryDequeue(out msg);
            message = msg;
            return result;
        }

        #endregion

        private void OnRequestLine(object sender, RequestLineEventArgs e)
        {
            _message = new HttpRequest(e.Verb, e.Url, e.HttpVersion);
        }

        private void OnHeaderComplete(object sender, EventArgs e)
        {
            _bodyBytestLeft = _message.ContentLength;
            if (_message.ContentLength == 0)
            {
                _messages.Enqueue(_message);
                _message = null;
                return;
            }

            if (_message.ContentLength > _bodySlice.Count)
                _bodyStream = new FileStream(Path.GetTempFileName(), FileMode.Create);
            else
                _bodyStream = new SliceStream(_bodySlice);
        }

        private void OnHeader(object sender, HeaderEventArgs e)
        {
            _message.AddHeader(e.Name, e.Value);
        }
    }
}