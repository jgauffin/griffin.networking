using System.Net;
using Griffin.Networking.Servers;

namespace BasicDemo
{
    public class MyServiceFactory : IServiceFactory
    {
        /// <summary>
        /// Create a new client
        /// </summary>
        /// <param name="remoteEndPoint">IP address of the remote end point</param>
        /// <returns>Created client</returns>
        public IServerService CreateClient(EndPoint remoteEndPoint)
        {
            return new MyService();
        }
    }
}