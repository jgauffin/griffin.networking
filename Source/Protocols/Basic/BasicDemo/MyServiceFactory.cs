using System.Net;
using Griffin.Networking.Servers;

namespace BasicDemo
{
    public class MyServiceFactory : IServiceFactory
    {
        #region IServiceFactory Members

        /// <summary>
        /// Create a new client
        /// </summary>
        /// <param name="remoteEndPoint">IP address of the remote end point</param>
        /// <returns>Created client</returns>
        public INetworkService CreateClient(EndPoint remoteEndPoint)
        {
            return new MyService();
        }

        #endregion
    }
}