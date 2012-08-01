using System.Collections.Generic;
using Griffin.Networking;

namespace Griffin.Networking.Protocol.FreeSwitch.Tests
{
    public class MyCtx : IPipelineHandlerContext
    {
        public List<IPipelineMessage> Upstream = new List<IPipelineMessage>();
        public List<IPipelineMessage> Downstream = new List<IPipelineMessage>();

        public void SendUpstream(IPipelineMessage message)
        {
            Upstream.Add(message);
        }

        public void SendDownstream(IPipelineMessage message)
        {
            Downstream.Add(message);
        }
    }
}