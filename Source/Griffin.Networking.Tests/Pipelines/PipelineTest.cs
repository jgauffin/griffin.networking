using System;
using Griffin.Networking.Messages;
using Griffin.Networking.Pipelines;
using Xunit;

namespace Griffin.Networking.Tests.Pipelines
{
    public class PipelineTest
    {
        [Fact]
        public void TestUpstreamOrder()
        {
            var pipeline = new Pipeline();
            pipeline.AddUpstreamHandler(new UpstreamOrderHandler(0));
            pipeline.AddUpstreamHandler(new UpstreamOrderHandler(1));
            pipeline.AddUpstreamHandler(new UpstreamOrderHandler(2));
            pipeline.SendUpstream(new MyMessage());
            Assert.Equal(3, UpstreamOrderHandler.CurrentIndex);
        }

        [Fact]
        public void TestDownstreamOrder()
        {
            var pipeline = new Pipeline();
            pipeline.AddDownstreamHandler(new DownstreamOrderHandler(2));
            pipeline.AddDownstreamHandler(new DownstreamOrderHandler(1));
            pipeline.AddDownstreamHandler(new DownstreamOrderHandler(0));
            pipeline.SendDownstream(new MyMessage());
            Assert.Equal(-1, DownstreamOrderHandler.CurrentIndex);
        }

        [Fact]
        public void SendUpStreamAndSwitchToDown()
        {
            var pipeline = new Pipeline();
            IPipelineMessage downMsg = null;
            pipeline.AddUpstreamHandler(new MyUpHandler((ctx, m) =>
                                                            {
                                                                var upMsg = m;
                                                                ctx.SendDownstream(upMsg);
                                                            }));
            pipeline.AddDownstreamHandler(new MyDownHandler((ctx, m) => downMsg = m));
            pipeline.SendUpstream(new Disconnected(new Exception()));
            Assert.NotNull(downMsg);

        }

    }
}
