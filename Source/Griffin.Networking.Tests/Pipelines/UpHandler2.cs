using System.Collections.Generic;
using Griffin.Networking.Pipelines;

namespace Griffin.Networking.Tests.Pipelines
{
    public class UpHandler2 : IUpstreamHandler
    {
        private readonly List<IPipelineMessage> Messages = new List<IPipelineMessage>();

        #region IUpstreamHandler Members

        public void HandleUpstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            Messages.Add(message);
        }

        #endregion
    }
}