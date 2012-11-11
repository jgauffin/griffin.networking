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
        private int _parserState = 0;

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
            if (char.IsWhiteSpace())
        }
    }
}