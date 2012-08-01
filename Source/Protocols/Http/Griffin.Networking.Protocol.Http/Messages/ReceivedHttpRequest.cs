using System;
using Griffin.Networking.Http.Protocol;
using Griffin.Networking.Http.Specification;

namespace Griffin.Networking.Http.Messages
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
