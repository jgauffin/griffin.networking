using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.Networking.JsonRpc.Messages
{
    /// <summary>
    /// Received a JSON Request.
    /// </summary>
    public class ReceivedRequest : IPipelineMessage
    {
        public Request Request { get; private set; }

        public ReceivedRequest(Request request)
        {
            Request = request;
        }
    }
}
