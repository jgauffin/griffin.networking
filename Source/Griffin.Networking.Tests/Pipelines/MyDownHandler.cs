using System;

namespace Griffin.Networking.Tests.Pipelines
{
    public class MyDownHandler : IDownstreamHandler
    {
        private readonly Action<IPipelineHandlerContext, IPipelineMessage> _action;

        public MyDownHandler(Action<IPipelineHandlerContext, IPipelineMessage> action)
        {
            _action = action;
        }

        public void HandleDownstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            _action(context, message);
        }
    }
}