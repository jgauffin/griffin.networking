using System.Text;
using Griffin.Networking.Buffers;
using Griffin.Networking.Protocol.Http.Implementation;
using Griffin.Networking.Protocol.Http.Protocol;
using Xunit;

namespace Griffin.Networking.Protocol.Http.Tests
{
    public class HttpHeaderParserTests
    {
        private readonly HttpHeaderParser _parser;
        private bool _completed;
        private IRequest _request;

        public HttpHeaderParserTests()
        {
            _parser = new HttpHeaderParser();
            _parser.RequestLineParsed +=
                (sender, args) => { _request = new HttpRequest(args.Verb, args.Url, args.HttpVersion); };
            _parser.HeaderParsed += (sender, args) => { _request.AddHeader(args.Name, args.Value); };
            _parser.Completed += (sender, args) => { _completed = true; };
        }

        [Fact]
        public void BasicDoc()
        {
            var slize = CreateSlice(@"GET / HTTP/1.1
Connection: Keep-Alive
HOST: localhost
Content-Length: 0

");
            _parser.Parse(new SliceStream(slize, slize.Count));

            Assert.Equal("Keep-Alive", _request.Headers["Connection"].Value);
            Assert.Equal("localhost", _request.Headers["HOST"].Value);
            Assert.Equal("0", _request.Headers["Content-Length"].Value);
        }

        [Fact]
        public void AddInSteps()
        {
            var slize = CreateSlice(@"GET / HTTP/1.1
Connection: Keep-Alive
HOST: localhost
Content-Length: 0

");
            var stream = new SliceStream(slize, 1);
            _parser.Parse(stream);

            stream.SetLength(10);
            _parser.Parse(stream);
            Assert.False(_completed);

            stream.SetLength(16);
            _parser.Parse(stream);
            Assert.False(_completed);

            stream.SetLength(78);
            _parser.Parse(stream);
            Assert.True(_completed);
            Assert.Equal("Keep-Alive", _request.Headers["Connection"].Value);
            Assert.Equal("localhost", _request.Headers["HOST"].Value);
            Assert.Equal("0", _request.Headers["Content-Length"].Value);
        }

        private BufferSlice CreateSlice(string doc)
        {
            var buffer = Encoding.ASCII.GetBytes(doc);
            return new BufferSlice(buffer, 0, buffer.Length);
        }
    }
}