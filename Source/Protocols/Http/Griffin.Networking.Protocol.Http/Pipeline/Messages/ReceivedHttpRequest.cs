using System;
using Griffin.Networking.Protocol.Http.Protocol;
using Griffin.Networking.Pipelines;

namespace Griffin.Networking.Protocol.Http.Pipeline.Messages
{
    /// <summary>
    /// Received a new http request.
    /// </summary>
    public class ReceivedHttpRequest : IPipelineMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivedHttpRequest" /> class.
        /// </summary>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <exception cref="System.ArgumentNullException">httpRequest</exception>
        public ReceivedHttpRequest(IRequest httpRequest)
        {
            if (httpRequest == null) throw new ArgumentNullException("httpRequest");
            HttpRequest = httpRequest;
        }

        /// <summary>
        /// Gets the received HTTP request.
        /// </summary>
        public IRequest HttpRequest { get; private set; }
    }
}