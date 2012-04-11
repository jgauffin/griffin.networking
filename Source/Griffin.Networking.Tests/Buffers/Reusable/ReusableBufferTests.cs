using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Networking.Buffers.Reusable;
using Xunit;

namespace Griffin.Networking.Tests.Buffers.Reusable
{

    public class ReusableBufferTests
    {
        

        [Fact]
        public void OutOfBuffers()
        {
            var bufferPool = new ByteBufferPool(65536, 1);
            bufferPool.Pop();
        }
    }
}
