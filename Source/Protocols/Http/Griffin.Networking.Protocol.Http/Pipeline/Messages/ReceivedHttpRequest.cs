using System;
using Griffin.Networking.Http.Protocol;
using Griffin.Networking.Pipelines;

namespace Griffin.Networking.Http.Messages
{
    /// <summary>
    /// Received a new http request.
    /// </summary>
    public class ReceivedHttpRequest : IPipelineMessage
    {
        public ReceivedHttpRequest(IRequest httpRequest)
        {
            if (httpRequest == null) throw new ArgumentNullException("httpRequest");
            HttpRequest = httpRequest;
        }

        public IRequest HttpRequest { get; private set; }
    }
}