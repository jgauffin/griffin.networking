using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Griffin.Networking.Messages
{
    /// <summary>
    /// bind socket for a server channel
    /// </summary>
    public class BindSocket : IPipelineMessage
    {
        private readonly IPEndPoint _endPoint;

        public BindSocket(IPEndPoint endPoint)
        {
            _endPoint = endPoint;
        }

        public IPEndPoint EndPoint
        {
            get {
                return _endPoint;
            }
        }
    }
}
