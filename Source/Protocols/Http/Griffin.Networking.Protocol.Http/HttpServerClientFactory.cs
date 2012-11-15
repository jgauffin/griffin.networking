using System.Net;
using Griffin.Networking.Servers;

namespace Griffin.Networking.Http
{
    public class HttpServerClientFactory : IServiceFactory
    {
        #region IServiceFactory Members

        /// <summary>
        /// Create a new client
        /// </summary>
        /// <param name="remoteEndPoint">IP address of the remote end point</param>
        /// <returns>Created client</returns>
        public IServerService CreateClient(EndPoint remoteEndPoint)
        {
            return null;
        }

        #endregion
    }
}