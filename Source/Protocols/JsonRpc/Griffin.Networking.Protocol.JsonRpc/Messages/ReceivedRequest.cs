using Griffin.Networking.Pipelines;

namespace Griffin.Networking.JsonRpc.Messages
{
    /// <summary>
    /// Received a JSON Request.
    /// </summary>
    public class ReceivedRequest : IPipelineMessage
    {
        public ReceivedRequest(Request request)
        {
            Request = request;
        }

        public Request Request { get; private set; }
    }
}