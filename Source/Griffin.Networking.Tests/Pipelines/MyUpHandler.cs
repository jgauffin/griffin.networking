using System;
using Griffin.Networking.Pipelines;

namespace Griffin.Networking.Tests.Pipelines
{
    public class MyUpHandler : IUpstreamHandler
    {
        private readonly Action<IPipelineHandlerContext, IPipelineMessage> _action;

        public MyUpHandler(Action<IPipelineHandlerContext, IPipelineMessage> action)
        {
            _action = action;
        }

        #region IUpstreamHandler Members

        public void HandleUpstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            _action(context, message);
        }

        #endregion
    }
}