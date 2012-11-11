using System.Net;

namespace Griffin.Networking.Servers
{
    /// <summary>
    /// Used to create the client (i.e. your class that handles a connection from a client)
    /// </summary>
    public interface IClientFactory
    {
        /// <summary>
        /// Create a new client
        /// </summary>
        /// <param name="remoteEndPoint">IP address of the remote end point</param>
        /// <returns>Created client</returns>
        IServerClient CreateClient(EndPoint remoteEndPoint);
    }
}