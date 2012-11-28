using System.Net;
using Griffin.Networking.Servers;

namespace Griffin.Networking.Protocol.Http.DemoServer.Basic
{
    /// <summary>
    /// this class will handle all incoming HTTP requests.
    /// </summary>
    public class MyHttpServiceFactory : IServiceFactory
    {
        #region IServiceFactory Members

        /// <summary>
        /// Create a new client
        /// </summary>
        /// <param name="remoteEndPoint">IP address of the remote end point</param>
        /// <returns>Created client</returns>
        public IServerService CreateClient(EndPoint remoteEndPoint)
        {
            return new MyHttpService();
        }

        #endregion
    }
}