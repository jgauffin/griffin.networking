using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Networking.Buffers;
using Xunit;

namespace Griffin.Networking.Protocol.FreeSwitch.Tests
{
    public class BufferSliceReaderTests
    {
        [Fact]
        public void TestSingleNewLine()
        {
            var buffer = Encoding.ASCII.GetBytes("Hello\nWorld!");
            var reader = new BufferSliceReader(buffer, 0, buffer.Length, Encoding.ASCII);

            var actual = reader.ReadLine();

            Assert.Equal("Hello", actual);
        }

    }
}
