using Griffin.Networking.Pipelines;
using Xunit;

namespace Griffin.Networking.Tests.Pipelines
{
    public class UpstreamOrderHandler : IUpstreamHandler
    {
        public static int CurrentIndex = 0;
        private readonly int _index;

        public UpstreamOrderHandler(int index)
        {
            if (index == 0)
                CurrentIndex = 0;
            _index = index;
        }

        #region IUpstreamHandler Members

        public void HandleUpstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            Assert.Equal(_index, CurrentIndex);
            CurrentIndex++;
            context.SendUpstream(message);
        }

        #endregion

        public override string ToString()
        {
            return _index.ToString();
        }
    }
}