using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Networking.Http.Protocol;
using Griffin.Networking.Http.Specification;

namespace Griffin.Networking.Http.Messages
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
