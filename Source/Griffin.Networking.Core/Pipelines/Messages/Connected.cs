using System.Net;

namespace Griffin.Networking.Pipelines.Messages
{
    /// <summary>
    /// Connected to an end point
    /// </summary>
    /// <remarks>
    /// Can be sent both in server and client channels to indicate that an connection have been established.
    /// </remarks>
    public class Connected : IPipelineMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Connected"/> class.
        /// </summary>
        /// <param name="remoteEndPoint">The remote end point.</param>
        public Connected(EndPoint remoteEndPoint)
        {
            RemoteEndPoint = remoteEndPoint;
        }

        /// <summary>
        /// Gets address of the remote end point
        /// </summary>
        public EndPoint RemoteEndPoint { get; private set; }
    }
}