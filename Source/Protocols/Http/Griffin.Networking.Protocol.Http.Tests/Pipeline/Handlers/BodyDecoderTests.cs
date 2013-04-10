using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Griffin.Networking.Buffers;
using Griffin.Networking.Pipelines;
using Griffin.Networking.Pipelines.Messages;
using Griffin.Networking.Protocol.Http.Implementation;
using Griffin.Networking.Protocol.Http.Pipeline.Handlers;
using Griffin.Networking.Protocol.Http.Pipeline.Messages;
using Griffin.Networking.Protocol.Http.Services;
using NSubstitute;
using Xunit;

namespace Griffin.Networking.Http.Tests.Pipeline.Handlers
{
    public class BodyDecoderTests
    {
        [Fact]
        public void NoRequestSet()
        {
            var service = Substitute.For<IBodyDecoder>();
            var received = new Received(new IPEndPoint(IPAddress.Loopback, 9231), Substitute.For<IBufferReader>());
            var context = Substitute.For<IPipelineHandlerContext>();

            var sut = new BodyDecoder(service, 65535, 65535);
            Assert.Throws<InvalidOperationException>(() => sut.HandleUpstream(context, received));
        }

        [Fact]
        public void Test()
        {
            var service = Substitute.For<IBodyDecoder>();
            var context = Substitute.For<IPipelineHandlerContext>();
            var request = new HttpRequest("GET", "/", "HTTP/1.1");
            request.Body = new MemoryStream(Encoding.ASCII.GetBytes("Hello world!"));
            var msg = new ReceivedHttpRequest(request);
            var sut  = new BodyDecoder(service, 65535, 65535);
            sut.HandleUpstream(context, msg);
            var received = new Received(new IPEndPoint(IPAddress.Loopback, 9231), Substitute.For<IBufferReader>());

            sut.HandleUpstream(context, received);
        }

    }
}
