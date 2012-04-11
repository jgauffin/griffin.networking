using System;

namespace Griffin.Networking.Tests.Pipelines
{
    public class MyUpHandler : IUpstreamHandler
    {
        private readonly Action<IPipelineHandlerContext, IPipelineMessage> _action;

        public MyUpHandler(Action<IPipelineHandlerContext, IPipelineMessage> action)
        {
            _action = action;
        }

        public void HandleUpstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            _action(context, message);
        }
    }
}