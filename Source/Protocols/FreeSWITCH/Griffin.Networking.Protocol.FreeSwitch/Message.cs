using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace Griffin.Networking.Protocol.FreeSwitch
{
    public class Message
    {
        private readonly MemoryStream _body = new MemoryStream();
        private readonly NameValueCollection _headers = new NameValueCollection();
        private int _contentLength = -1;

        public Stream Body
        {
            get { return _body; }
        }

        public int ContentLength
        {
            get
            {
                if (_contentLength == -1)
                {
                    var header = _headers["Content-Length"];
                    if (header != null)
                        _contentLength = int.Parse(header);
                }
                return _contentLength;
            }
        }

        public NameValueCollection Headers
        {
            get { return _headers; }
        }

        public int Append(byte[] buffer, int offset, int count)
        {
            var bytesLeft = ContentLength - _body.Position;
            var bytesToUse = (int) Math.Min(bytesLeft, count);
            _body.Write(buffer, offset, bytesToUse);
            return bytesToUse;
        }

        public void Reset()
        {
            _body.SetLength(0);
            _body.Position = 0;
            _headers.Clear();
            _contentLength = -1;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (string key in Headers)
                sb.AppendFormat("{0}: {1}\n", key, _headers[key]);
            return sb.ToString();
        }
    }
}