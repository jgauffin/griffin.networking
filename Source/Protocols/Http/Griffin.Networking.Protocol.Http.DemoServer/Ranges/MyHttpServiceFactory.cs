using System.Net;
using Griffin.Networking.Servers;

namespace Griffin.Networking.Protocol.Http.DemoServer.Ranges
{
    public class MyHttpServiceFactory : IServiceFactory
    {
        public IServerService CreateClient(EndPoint remoteEndPoint)
        {
            return new MyHttpService();
        }
    }
}