using System.Collections.Generic;
using Griffin.Networking.Pipelines;

namespace Griffin.Networking.Tests.Pipelines
{
    public class UpHandler2 : IUpstreamHandler
    {
        List<IPipelineMessage>  Messages = new List<IPipelineMessage>();
        public void HandleUpstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            Messages.Add(message);
        }
    }
}