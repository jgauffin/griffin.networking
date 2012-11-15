using Griffin.Networking.Buffers;
using Xunit;

namespace Griffin.Networking.Tests.Buffers.Reusable
{
    public class BufferSliceStackTests
    {
        [Fact]
        public void CreateBuffer()
        {
            var bufferPool = new BufferSliceStack(1, 10);
            var buffer = bufferPool.Pop();
            Assert.NotNull(buffer);
            Assert.Equal(10, buffer.Count);
        }

        [Fact]
        public void Overflow()
        {
            var bufferPool = new BufferSliceStack(1, 10);

            bufferPool.Pop();
            bufferPool.Pop();
        }
    }
}