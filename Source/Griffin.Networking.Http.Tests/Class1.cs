using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Networking.Buffers;
using Griffin.Networking.Http.Implementation;
using Xunit;

namespace Griffin.Networking.Http.Tests
{
    public class TestParser
    {
        [Fact]
        public void BasicDoc()
        {
            var slize = CreateSlice(@"GET / HTTP/1.1
Connection: Keep-Alive
HOST: localhost
Content-Length: 0

");
            HttpParser parser = new HttpParser();
            var request = parser.Parse(slize);
            Assert.Equal("Keep-Alive", request.Headers["Connection"].Value);
            Assert.Equal("localhost", request.Headers["HOST"].Value);
            Assert.Equal("0", request.Headers["Content-Length"].Value);
        }

        [Fact]
        public void AddInSteps()
        {
            var slize = CreateSlice(@"GET / HTTP/1.1
Connection: Keep-Alive
HOST: localhost
Content-Length: 0

");
            slize.Count = 1;
            HttpParser parser = new HttpParser();
            Assert.Null(parser.Parse(slize));
            Assert.Equal(0, slize.Position);

            slize.Count = 10;
            Assert.Null(parser.Parse(slize));
            Assert.Equal(0, slize.Position);

            //first line
            slize.Count = 16;
            Assert.Null(parser.Parse(slize));
            Assert.Equal(16, slize.Position);

            slize.Count = 78 - 16;
            var request = parser.Parse(slize);
            Assert.Equal("Keep-Alive", request.Headers["Connection"].Value);
            Assert.Equal("localhost", request.Headers["HOST"].Value);
            Assert.Equal("0", request.Headers["Content-Length"].Value);
        }

        private BufferSlice CreateSlice(string doc)
        {
            var buffer = Encoding.ASCII.GetBytes(doc);
            return new BufferSlice(buffer, 0, buffer.Length, buffer.Length);
        }
    }
}
