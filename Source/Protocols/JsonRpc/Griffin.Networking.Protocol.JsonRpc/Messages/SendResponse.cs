using System;
using Griffin.Networking.Pipelines;

namespace Griffin.Networking.JsonRpc.Messages
{
    public class SendResponse : IPipelineMessage
    {
        public SendResponse(ResponseBase response)
        {
            if (response == null) throw new ArgumentNullException("response");
            Response = response;
        }

        public ResponseBase Response { get; private set; }
    }
}