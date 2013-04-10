using System.Net;
using Griffin.Networking.Servers;

namespace Griffin.Networking.Protocol.Http.DemoServer.Ranges
{
    public class MyHttpServiceFactory : IServiceFactory
    {
        public INetworkService CreateClient(EndPoint remoteEndPoint)
        {
            return new MyHttpService();
        }
    }
}