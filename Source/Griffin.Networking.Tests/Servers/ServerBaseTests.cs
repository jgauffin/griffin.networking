using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using FluentAssertions;
using Griffin.Networking.Servers;
using Xunit;

namespace Griffin.Networking.Tests.Servers
{
    public class ServerBaseTests
    {
        private MyTest _actual;

        public class MyTest : INetworkService
    {
            private IServerClientContext _context;

            public void Dispose()
            {
                
            }

            public void Assign(IServerClientContext context)
            {
                _context = context;
            }

            public void HandleReceive(object message)
            {
            }

            public void OnUnhandledException(ServiceExceptionContext context)
            {
            }
    }
        public class MyServer : ServerBase
        {
            private readonly Func<INetworkService> _factory;

            public MyServer(ServerConfiguration configuration, Func<INetworkService> factory) : base(configuration)
            {
                _factory = factory;
            }

            protected override INetworkService CreateClient(EndPoint remoteEndPoint)
            {
                return _factory();

            }
        }

        [Fact]
        public void Fix()
        {
            MyTest client = null;
            var myServer = new MyServer(new ServerConfiguration(), () =>
                {
                    client = new MyTest();
                    return client;
                });
            myServer.Start(new IPEndPoint(IPAddress.Any, 0));
            myServer.Stop();
            myServer.Start(new IPEndPoint(IPAddress.Any, 0));

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(IPAddress.Loopback, myServer.LocalPort);
            Thread.Sleep(500);
            myServer.Stop();

            client.Should().NotBeNull();
            myServer.Stop();
        }
    }
}
