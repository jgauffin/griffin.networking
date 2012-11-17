using System;
using Griffin.Networking.Http.Protocol;
using Griffin.Networking.Pipelines;

namespace Griffin.Networking.Http.Pipeline.Messages
{
    /// <summary>
    /// Sned a new HTTP resposne
    /// </summary>
    public class SendHttpResponse : IPipelineMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendHttpResponse"/> class.
        /// </summary>
        /// <param name="request">The request that the response is for.</param>
        /// <param name="response">Response to send.</param>
        public SendHttpResponse(IRequest request, IResponse response)
        {
            if (response == null) throw new ArgumentNullException("response");
            Request = request;
            Response = response;
        }

        /// <summary>
        /// Gets request that the response is for
        /// </summary>
        public IRequest Request { get; private set; }

        /// <summary>
        /// Gets actual response
        /// </summary>
        public IResponse Response { get; private set; }
    }
}