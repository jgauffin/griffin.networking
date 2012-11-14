using Griffin.Networking.Http.Protocol;
using Griffin.Networking.Pipelines;

namespace Griffin.Networking.Http.Messages
{
    public class SendHttpResponse : IPipelineMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendHttpResponse"/> class.
        /// </summary>
        /// <param name="request">The request that the response is for.</param>
        /// <param name="response">Response to send.</param>
        public SendHttpResponse(IRequest request, IResponse response)
        {
            Request = request;
            Response = response;
        }

        public IRequest Request { get; private set; }
        public IResponse Response { get; private set; }
    }
}