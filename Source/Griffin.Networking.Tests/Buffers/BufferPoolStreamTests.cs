using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Networking.Buffers;
using Xunit;

namespace Griffin.Networking.Tests.Buffers
{
    public class BufferPoolStreamTests
    {
        [Fact]
        public void PartialBuffeR()
        {
            var buffer = new byte[512];
            var stream = new BufferPoolStream(new BufferSlice(buffer, 256, 256, 0));
        }
    }
}
