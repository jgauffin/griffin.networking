using System;
using Griffin.Networking.Protocol.Http.Protocol;

namespace Griffin.Networking.Protocol.Http.Implementation
{
    /// <summary>
    /// Parses a request cookie header value.
    /// </summary>
    /// <remarks>This class is not thread safe.</remarks>
    public class HttpCookieParser
    {
        private readonly string _headerValue;
        private HttpCookieCollection<IHttpCookie> _cookies;
        private int _index;
        private string _cookieName = "";
        private Action _parserMethod;
        private string _cookieValue = "";


        /// <summary>
        /// Initializes a new instance of the <see cref="HttpCookieParser" /> class.
        /// </summary>
        /// <param name="headerValue">The header value.</param>
        public HttpCookieParser(string headerValue)
        {
            if (headerValue == null) throw new ArgumentNullException("headerValue");
            _headerValue = headerValue;
        }

        private char Current
        {
            get
            {
                if (_index >= _headerValue.Length)
                    return char.MinValue;

                return _headerValue[_index];
            }
        }

        protected bool IsEOF
        {
            get { return _index >= _headerValue.Length; }
        }

        protected void Name_Before()
        {
            while (char.IsWhiteSpace(Current))
            {
                MoveNext();
            }

            _parserMethod = Name;
        }

        protected virtual void Name()
        {
            while (!char.IsWhiteSpace(Current) && Current != '=')
            {
                _cookieName += Current;
                MoveNext();
            }

            _parserMethod = Name_After;
        }

        protected virtual void Name_After()
        {
            while (char.IsWhiteSpace(Current) || Current == ':')
            {
                MoveNext();
            }

            _parserMethod = Value_Before;
        }

        protected virtual void Value_Before()
        {
            if (Current == '"')
                _parserMethod = Value_Qouted;
            else
                _parserMethod = Value;

            MoveNext();
        }

        private void Value()
        {
            while (Current != ';' && !IsEOF)
            {
                _cookieValue += Current;
                MoveNext();
            }

            _parserMethod = Value_After;
        }

        private void Value_Qouted()
        {
            MoveNext(); // skip '"'

            var last = char.MinValue;
            while (Current != '"' && !IsEOF)
            {
                if (Current == '"' && last == '\\')
                {
                    _cookieValue += '#';
                    MoveNext();
                }
                else
                {
                    _cookieValue += Current;
                }

                last = Current;
                MoveNext();
            }

            _parserMethod = Value_After;
        }

        private void Value_After()
        {
            OnCookie(_cookieName, _cookieValue);
            _cookieName = "";
            _cookieValue = "";
            while (char.IsWhiteSpace(Current) || Current == ';')
            {
                MoveNext();
            }

            _parserMethod = Name_Before;
        }

        private void OnCookie(string name, string value)
        {
            if (name == null) throw new ArgumentNullException("name");

            _cookies.Add(new HttpCookie(name, value));
        }

        private void MoveNext()
        {
            if (!IsEOF)
                ++_index;
        }

        /// <summary>
        /// Parse cookie string
        /// </summary>
        /// <returns>A generated cookie collection.</returns>
        public IHttpCookieCollection<IHttpCookie> Parse()
        {
            _cookies = new HttpCookieCollection<IHttpCookie>();
            _parserMethod = Name_Before;

            while (!IsEOF)
            {
                _parserMethod();
            }

            OnCookie(_cookieName, _cookieValue);
            return _cookies;
        }
    }
}