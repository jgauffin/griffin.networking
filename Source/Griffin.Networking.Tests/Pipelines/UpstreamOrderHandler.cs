using Griffin.Networking.Pipelines;
using Xunit;

namespace Griffin.Networking.Tests.Pipelines
{
    public class UpstreamOrderHandler : IUpstreamHandler
    {
        private readonly int _index;
        public static int CurrentIndex = 0;
        public UpstreamOrderHandler(int index)
        {
            if (index == 0)
                CurrentIndex = 0;
            _index = index;
        }

        public void HandleUpstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            Assert.Equal(_index, CurrentIndex);
            CurrentIndex++;
            context.SendUpstream(message);
        }

        public override string ToString()
        {
            return _index.ToString();
        }
    }
}