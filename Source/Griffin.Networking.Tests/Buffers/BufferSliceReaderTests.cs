using System.Text;
using Griffin.Networking.Buffers;
using Xunit;

namespace Griffin.Networking.Tests.Buffers
{
    public class BufferSliceReaderTests
    {
        [Fact]
        public void TestInit()
        {
            var buffer = Encoding.ASCII.GetBytes("Hello world.!");
            var slice = new BufferSlice(buffer, 0, buffer.Length, buffer.Length);
            var reader = new BufferSliceReader(slice);

            Assert.Equal(slice.Count, reader.Length);
            Assert.Equal(slice.RemainingLength, reader.RemainingLength);
            Assert.Equal('H', reader.Current);
            Assert.Equal('e', reader.Peek);
            Assert.True(reader.HasMore);
        }

        [Fact]
        public void TestConsume()
        {
            var buffer = Encoding.ASCII.GetBytes("Hello world.!");
            var slice = new BufferSlice(buffer, 0, buffer.Length, buffer.Length); 

            var reader = new BufferSliceReader(slice);

            reader.Consume();
            Assert.Equal(slice.Count, reader.Length);
            Assert.Equal(slice.RemainingLength, reader.RemainingLength);
            Assert.Equal('e', reader.Current);
            Assert.Equal('l', reader.Peek);
            Assert.True(reader.HasMore);
        }

        [Fact]
        public void TestConsumeHe()
        {
            var buffer = Encoding.ASCII.GetBytes("Hello world.!");
            var slice = new BufferSlice(buffer, 0, buffer.Length, buffer.Length);
            var reader = new BufferSliceReader(slice);

            reader.Consume('H', 'e');
            Assert.Equal('l', reader.Current);
            Assert.Equal('l', reader.Peek);
        }

        [Fact]
        public void TestConsumeUntilSpace()
        {
            var buffer = Encoding.ASCII.GetBytes("Hello world.!");
            var slice = new BufferSlice(buffer, 0, buffer.Length, buffer.Length);
            var reader = new BufferSliceReader(slice);

            reader.ConsumeUntil(' ');
            Assert.Equal(' ', reader.Current);
            Assert.Equal('w', reader.Peek);
        }

        [Fact]
        public void TestConsumeUntilSpaceAndWhiteSpaces()
        {
            var buffer = Encoding.ASCII.GetBytes("Hello  \tworld.!");
            var slice = new BufferSlice(buffer, 0, buffer.Length, buffer.Length);
            var reader = new BufferSliceReader(slice);

            reader.ConsumeUntil(' ');
            reader.ConsumeWhiteSpaces();
            Assert.Equal('w', reader.Current);
            Assert.Equal('o', reader.Peek);
        }

        [Fact]
        public void TestContains()
        {
            var buffer = Encoding.ASCII.GetBytes("Hello  \tworld.!");
            var slice = new BufferSlice(buffer, 0, buffer.Length, buffer.Length);
            var reader = new BufferSliceReader(slice);

            reader.Contains(' ');
            Assert.Equal('H', reader.Current);
            Assert.Equal('e', reader.Peek);
        }

        [Fact]
        public void TestReadChar()
        {
            var buffer = Encoding.ASCII.GetBytes("Hello  \tworld.!");
            var slice = new BufferSlice(buffer, 0, buffer.Length, buffer.Length);
            var reader = new BufferSliceReader(slice);

            Assert.Equal('H', reader.Read());
            Assert.Equal('e', reader.Current);
            Assert.Equal('l', reader.Peek);
        }

        [Fact]
        public void TestConsumeEnd()
        {
            var buffer = Encoding.ASCII.GetBytes("Hello  \tworld.!");
            var slice = new BufferSlice(buffer, 0, buffer.Length, buffer.Length);
            var reader = new BufferSliceReader(slice);

            reader.ReadUntil('!');
            Assert.Equal('!', reader.Current);
            reader.Consume();
            Assert.True(reader.EndOfFile);
            Assert.Equal(0, reader.RemainingLength);
        }
    }
}
