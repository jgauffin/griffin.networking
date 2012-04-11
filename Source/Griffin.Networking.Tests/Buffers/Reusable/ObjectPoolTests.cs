using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Networking.Buffers.Reusable;
using Xunit;

namespace Griffin.Networking.Tests.Buffers.Reusable
{
    public class ByteBufferPoolTests
    {
        

        [Fact]
        public void IncreaseBuffer()
        {
            var bufferPool = new ByteBufferPool(65536, 1);
            var buffer=bufferPool.Pop();
            Assert.NotNull(buffer);
            Assert.Equal(65536,buffer.Length);
        }
    }
}
