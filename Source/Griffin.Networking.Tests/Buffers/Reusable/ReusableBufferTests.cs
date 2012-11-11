using System;
using Griffin.Networking.Buffers;
using Xunit;

namespace Griffin.Networking.Tests.Buffers.Reusable
{

    public class ReusableBufferTests
    {
        [Fact]
        public void Pop_Return_Pop()
        {
            var bufferPool = new BufferSliceStack(1, 100);

            var slice = bufferPool.Pop();
            Assert.Throws<InvalidOperationException>(() => bufferPool.Pop());
            ((PooledBufferSlice) slice).Dispose();
            var slice2 = bufferPool.Pop();

            Assert.Same(slice, slice2);
        }
    }
}
