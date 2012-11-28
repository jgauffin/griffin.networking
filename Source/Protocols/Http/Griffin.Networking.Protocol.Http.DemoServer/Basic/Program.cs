using System.Net;
using Griffin.Networking.Messaging;

namespace Griffin.Networking.Protocol.Http.DemoServer.Basic
{
    internal class Program
    {
        public static void RunDemo()
        {
            var server = new MessagingServer(new MyHttpServiceFactory(),
                                             new MessagingServerConfiguration(new HttpMessageFactory()));
            server.Start(new IPEndPoint(IPAddress.Loopback, 8888));
        }
    }
}