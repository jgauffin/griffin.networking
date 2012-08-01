using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.Networking.JsonRpc.Messages
{
    public class SendResponse : IPipelineMessage
    {
        public ResponseBase Response { get; private set; }

        public SendResponse(ResponseBase response)
        {
            if (response == null) throw new ArgumentNullException("response");
            Response = response;
        }
    }
}
