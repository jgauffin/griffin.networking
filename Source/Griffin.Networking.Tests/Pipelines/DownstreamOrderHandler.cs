using Griffin.Networking.Pipelines;
using Xunit;

namespace Griffin.Networking.Tests.Pipelines
{
    public class DownstreamOrderHandler : IDownstreamHandler
    {
        public static int CurrentIndex = 2;
        private readonly int _index;

        public DownstreamOrderHandler(int index)
        {
            if (index == 0)
                CurrentIndex = 2;
            _index = index;
        }

        #region IDownstreamHandler Members

        public void HandleDownstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            Assert.Equal(_index, CurrentIndex);
            CurrentIndex--;
            context.SendDownstream(message);
        }

        #endregion

        public override string ToString()
        {
            return _index.ToString();
        }
    }
}