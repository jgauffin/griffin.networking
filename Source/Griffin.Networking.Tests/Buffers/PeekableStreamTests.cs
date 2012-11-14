using System;
using System.IO;
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
            var slice = new BufferSlice(initialBuffer, 0, initialBuffer.Length);
            var stream = new SliceStream(slice, initialBuffer.Length);
            Assert.Equal(initial.Length, stream.Length);
            Assert.Equal(initial.Length, ((IBufferWrapper) stream).Capacity);
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
            var slice = new BufferSlice(buffer, 0, buffer.Length);
            var stream = new SliceStream(slice, text.Length);
            stream.SetLength(initial.Length);
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