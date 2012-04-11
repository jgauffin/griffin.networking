using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Networking.Protocols.Http.Specification;

namespace Griffin.Networking.Protocols.Http.Messages
{
    public class SendHttpResponse : IPipelineMessage
    {
        public IResponse Response { get; set; }

        public SendHttpResponse(IResponse response)
        {
            Response = response;
        }
    }
}
