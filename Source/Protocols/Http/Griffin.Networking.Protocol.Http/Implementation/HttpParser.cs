using System;
using System.Net;
using Griffin.Networking.Buffers;
using Griffin.Networking.Http.Handlers;
using Griffin.Networking.Http.Pipeline.Handlers;
using Griffin.Networking.Http.Protocol;
using Griffin.Networking.Logging;

namespace Griffin.Networking.Http.Implementation
{
    /// <summary>
    /// A HTTP parser implementation.
    /// </summary>
    public class HttpParser : IHttpParser
    {
        private IStringBufferReader _reader;
        private int _bodyBytesLeft;
        private string _headerName;
        private string _headerValue;
        private bool _isComplete;
        private IMessage _message;
        private Func<bool> _parserMethod;
        private ILogger _logger = LogManager.GetLogger<HttpParser>();


        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderDecoder"/> class.
        /// </summary>
        public HttpParser()
        {
            _parserMethod = ParseFirstLine;
        }

        public IMessage Parse(IStringBufferReader reader)
        {
            _reader = reader;
            _logger.Trace("Parsing method: " + _parserMethod.Method.Name);
            while (_parserMethod())
            {
                _logger.Trace("Next parsing method: " + _parserMethod.Method.Name);
            }
                

            if (_isComplete)
                return _message;

            return null;
        }

        /// <summary>
        /// Try to find a header name.
        /// </summary>
        /// <returns></returns>
        private bool GetHeaderName()
        {
            // empty line. body is begining.
            if (_reader.Current == '\r' && _reader.Peek == '\n')
            {
                // Eat the line break
                _reader.Consume('\r', '\n');

                _isComplete = true;
                _parserMethod = ParseFirstLine;
                return true;
            }

            _headerName = _reader.ReadUntil(':');
            if (_headerName == null)
                return false;

            _reader.Consume(); // eat colon
            _parserMethod = GetHeaderValue;
            return true;
        }

        /// <summary>
        /// Get header values.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Will also look for multi header values and automatically merge them to one line.</remarks>
        private bool GetHeaderValue()
        {
            // remove white spaces.
            _reader.Consume(' ', '\t');

            // multi line or empty value?
            if (_reader.Current == '\r' && _reader.Peek == '\n')
            {
                _reader.Consume('\r', '\n');

                // empty value.
                if (_reader.Current != '\t' && _reader.Current != ' ')
                {
                    OnHeader(_headerName, string.Empty);
                    _headerName = null;
                    _headerValue = string.Empty;
                    _parserMethod = GetHeaderName;
                    return true;
                }

                if (_reader.RemainingLength < 1)
                    return false;

                // consume one whitespace
                _reader.Consume();

                // and fetch the rest.
                return GetHeaderValue();
            }

            var value = _reader.ReadLine();
            if (value == null)
                return false;

            _headerValue += value;
            if (_headerName.Equals("Content-Length", StringComparison.OrdinalIgnoreCase))
            {
                if (!int.TryParse(value, out _bodyBytesLeft))
                {
                    throw new HttpException(HttpStatusCode.BadRequest, "Content length is not a number: " + value);
                }
            }

            OnHeader(_headerName, _headerValue);

            _headerName = null;
            _headerValue = string.Empty;
            _parserMethod = GetHeaderName;
            return true;
        }

        /// <summary>
        /// First message line.
        /// </summary>
        /// <param name="words">Will always contain three elements.</param>
        protected virtual void OnFirstLine(string[] words)
        {
            var firstWord = words[0].ToUpper();

            _message = firstWord.StartsWith("HTTP")
                           ? CreateResponse(words[0], words[1], words[2])
                           : CreateRequest(words[0], words[1], words[2]);
        }

        private IMessage CreateRequest(string httpMethod, string uri, string httpVersion)
        {
            return new HttpRequest(httpMethod, uri, httpVersion);
        }

        private IMessage CreateResponse(string httpVersion, string code, string reason)
        {
            return new HttpResponse(httpVersion, int.Parse(code), reason);
        }

        private void OnHeader(string name, string value)
        {
            _message.AddHeader(name, value);
        }

        /// <summary>
        /// Parses the first line in a request/response.
        /// </summary>
        /// <returns><c>true</c> if first line is well formatted; otherwise <c>false</c>.</returns>
        private bool ParseFirstLine()
        {
            _reader.Consume('\r', '\n');

            // Do not contain a complete first line.
            if (!_reader.Contains('\n'))
                return false;

            var words = new string[3];
            words[0] = _reader.ReadUntil(' ');
            _reader.Consume(); // eat delimiter
            words[1] = _reader.ReadUntil(' ');
            _reader.Consume(); // eat delimiter
            words[2] = _reader.ReadLine();
            if (string.IsNullOrEmpty(words[0]) || string.IsNullOrEmpty(words[1]) || string.IsNullOrEmpty(words[2]))
            {
                throw new HttpException(HttpStatusCode.BadRequest, "Invalid request line: " + string.Join(" ", words));
            }

            OnFirstLine(words);
            _parserMethod = GetHeaderName;
            return true;
        }


        /// <summary>
        /// Reset parser to initial state.
        /// </summary>
        public void Reset()
        {
            _headerValue = null;
            _headerName = string.Empty;
            _bodyBytesLeft = 0;
            _parserMethod = ParseFirstLine;
        }
    }
}