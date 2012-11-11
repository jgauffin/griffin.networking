using System;
using System.Text;
using Griffin.Networking.Buffers;

namespace Griffin.Networking.Http.Pipeline.Handlers
{
    public class MyHttpParser
    {
        private Action<char> _parserMethod;
        private StringBuilder _headerName = new StringBuilder();
        private StringBuilder _headerValue = new StringBuilder();

        public void Parse(IBufferReader reader)
        {
            var theByte = 0;
            while ((theByte = reader.Read()) != -1)
            {
                var ch = (char) theByte;
                _parserMethod(ch);
            }
        }

        public void ParseHeaderName(char ch)
        {
            if (char.IsWhiteSpace(ch))
                return;

            if (ch == ':')
                _parserMethod = ParserHeaderValue;

            _headerName.Append(ch);
        }

        private void ParserHeaderValue(char ch)
        {
            if (ch == '"')
            {
                _parserMethod = ParseQuotedHeaderValue;
                return;
            }

            if (char.IsWhiteSpace(ch) || ch == '\r')
                return;

            if (ch == '\n')
                _parserMethod = MultilineOrHeaderName;

            _headerValue.Append(ch);
        }

        private void ParseQuotedHeaderValue(char ch)
        {
            if (ch == '\"')
            {
                _parserMethod = ParseQuotedHeaderValue;
                return;
            }

            _headerValue.Append(ch);
        }

        private void MultilineOrHeaderName(char ch)
        {
            if (IsHorizontalWhitespace(ch))
            {
                _parserMethod = ParserHeaderValue;
                return;
            }
            if (ch == '\r')
                return; //empty line
            if(ch== '\n')
            {
                //Header completed
                HeaderCompleted(this, EventArgs.Empty);
            }


            _parserMethod = ParseHeaderName;
            _args.Set(_headerName.ToString(), _headerValue.ToString());
            _headerName.Clear();
            _headerValue.Clear();
            HeaderParsed(this, _args);
        }

        private HeaderEventArgs _args = new HeaderEventArgs();
        public event EventHandler HeaderCompleted = delegate { };

        private bool IsHorizontalWhitespace(char ch)
        {
            return ch == ' ' || ch == '\t';
        }

        public event EventHandler<HeaderEventArgs> HeaderParsed = delegate { };

        public void Reset()
        {
            _headerName.Clear();
            _headerValue.Clear();
        }
    }

    public class HeaderEventArgs : EventArgs
    {
        public void Set(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Value { get; private set; }

        public string Name { get; private set; }
    }
}