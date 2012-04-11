using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Griffin.Networking.Messages
{
    /// <summary>
    /// Connect to a server
    /// </summary>
    public class Connect : IPipelineMessage
    {
        private readonly EndPoint _remoteEndPoint;

        public Connect(EndPoint remoteEndPoint)
        {
            if (remoteEndPoint == null)
                throw new ArgumentNullException("remoteEndPoint");

            _remoteEndPoint = remoteEndPoint;
        }

        /// <summary>
        /// Gets end point to connect to
        /// </summary>
        public EndPoint RemoteEndPoint
        {
            get { return _remoteEndPoint; }
        }
    }
}
