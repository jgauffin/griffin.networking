using System;
using Griffin.Networking.Protocols.Http.Specification;

namespace Griffin.Networking.Protocols.Http.Messages
{
    /// <summary>
    /// Received a new http request.
    /// </summary>
    public class ReceivedHttpRequest : IPipelineMessage
    {
        public IRequest HttpRequest { get; private set; }

        public ReceivedHttpRequest(IRequest httpRequest)
        {
            if (httpRequest == null) throw new ArgumentNullException("httpRequest");
            HttpRequest = httpRequest;
        }
    }
}
