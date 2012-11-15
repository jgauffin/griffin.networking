using Griffin.Networking.Pipelines;
using Xunit;

namespace Griffin.Networking.Tests.Pipelines
{
    public class PipelineTests
    {
        [Fact]
        public void SendDownstream()
        {
            var factory = new ServiceLocatorPipelineFactory(null);
            //var pipeline = factory.Create();
        }
    }
}