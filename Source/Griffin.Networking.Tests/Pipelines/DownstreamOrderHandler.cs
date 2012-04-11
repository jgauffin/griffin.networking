using Xunit;

namespace Griffin.Networking.Tests.Pipelines
{
    public class DownstreamOrderHandler : IDownstreamHandler
    {
        private readonly int _index;
        public static int CurrentIndex = 2;
        public DownstreamOrderHandler(int index)
        {
            if (index == 0)
                CurrentIndex = 2;
            _index = index;
        }

        public void HandleDownstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            Assert.Equal(_index, CurrentIndex);
            CurrentIndex--;
            context.SendDownstream(message);
        }

        public override string ToString()
        {
            return _index.ToString();
        }
    }
}