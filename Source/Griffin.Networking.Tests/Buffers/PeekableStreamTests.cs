using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Griffin.Networking.Buffers;
using Xunit;

namespace Griffin.Networking.Tests.Buffers
{
    public class PeekableStreamTests
    {
        [Fact]
        public void InitUsingByteArray()
        {
            var initial = "Hello world!";
            var initialBuffer = Encoding.ASCII.GetBytes(initial);
            var stream = new PeekableStream(initialBuffer, 0, initialBuffer.Length, initialBuffer.Length);
            Assert.Equal(initial.Length, stream.Length);
            Assert.Equal(initial.Length, stream.Capacity);
            Assert.Equal(0, stream.Position);
        }

        [Fact]
        public void InitUsingWrittenAndWriteMore()
        {
            var initial = "Hello world!";
            var addition = "Something more..";
            var buffer = new byte[65535];
            var text = Encoding.ASCII.GetBytes(initial);
            Buffer.BlockCopy(text, 0, buffer, 0, text.Length);
            var stream = new PeekableStream(buffer, 0, buffer.Length, text.Length);
            stream.Position = stream.Length;

            var writeable = Encoding.ASCII.GetBytes(addition);
            stream.Write(writeable, 0, writeable.Length);

            stream.Position = 0;
            var reader = new StreamReader(stream);
            var actual = reader.ReadToEnd();
            Assert.Equal(initial + addition, actual);

        }
    }
}
