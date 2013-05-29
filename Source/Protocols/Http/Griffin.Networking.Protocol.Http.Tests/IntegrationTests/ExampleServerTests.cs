using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Griffin.Networking.Buffers;
using Griffin.Networking.Messaging;
using Griffin.Networking.Protocol.Http;
using Griffin.Networking.Protocol.Http.Protocol;
using Griffin.Networking.Servers;
using Xunit;

namespace Griffin.Networking.Http.Tests.IntegrationTests
{
    public class ExampleServerTests
    {
        [Fact]
        public void Run()
        {
                var server = new MessagingServer(new MyHttpServiceFactory(),
                                                    new MessagingServerConfiguration(new HttpMessageFactory()));
                server.Start(new IPEndPoint(IPAddress.Loopback, 8888));

            Thread.Sleep(100000);

            }

        // factory
        public class MyHttpServiceFactory : IServiceFactory
        {
            public INetworkService CreateClient(EndPoint remoteEndPoint)
            {
                return new MyHttpService();
            }
        }

        // and the handler
        public class MyHttpService : HttpService
        {
            private static readonly BufferSliceStack Stack = new BufferSliceStack(50, 32000);

            public MyHttpService()
                : base(Stack)
            {
            }

            public override void Dispose()
            {
            }

            public override void OnRequest(IRequest request)
            {
                var response = request.CreateResponse(HttpStatusCode.OK, "Welcome");

                response.Body = new MemoryStream();
                response.ContentType = "text/plain";
                var buffer = Encoding.UTF8.GetBytes(request.RemoteEndPoint.Address.ToString());
                response.Body.Write(buffer, 0, buffer.Length);
                response.Body.Position = 0;

                Send(response);
            }
        }
    }
}
