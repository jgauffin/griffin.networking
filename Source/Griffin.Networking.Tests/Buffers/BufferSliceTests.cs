using System;
using System.Text;
using Griffin.Networking.Buffers;
using Xunit;

namespace Griffin.Networking.Tests.Buffers
{
    public class BufferSliceTests
    {
        [Fact]
        public void InitWithWrittenBuffer()
        {
            var buffer = Encoding.ASCII.GetBytes("Hello world");
            var slice = new BufferSlice(buffer, 0, buffer.Length, buffer.Length);
            Assert.Equal(buffer.Length, slice.Count);
            Assert.Equal(buffer.Length, slice.Capacity);

            // position = 0
            Assert.Equal(buffer.Length, slice.RemainingLength);
        }

        [Fact]
        public void Create()
        {
            var slice = new byte[65535];
            var buffer = new BufferSlice(slice, 0, slice.Length, slice.Length);
            Assert.Equal(65535, buffer.Capacity);
            Assert.Equal(0, buffer.Position);
            Assert.Equal(0, buffer.StartOffset);
            Assert.Equal(65535, buffer.RemainingCapacity);

            // since offset = 0
            Assert.Equal(65535, buffer.RemainingLength);

            Assert.Equal(65535, buffer.Count);
        }



        [Fact]
        public void ReadSome()
        {
            var slice = new byte[65535];
            var buffer = new BufferSlice(slice, 0, slice.Length, slice.Length);
            buffer.Position += 10;
            Assert.Equal(65535, buffer.Capacity);
            Assert.Equal(10, buffer.Position);
            Assert.Equal(0, buffer.StartOffset);
            Assert.Equal(65525, buffer.RemainingCapacity);
        }

        [Fact]
        public void SlicedBuffer()
        {
            var slice = new byte[65535];
            var buffer = new BufferSlice(slice, 32768, 32768, 32768);
            Assert.Equal(32768, buffer.Capacity);
            Assert.Equal(32768, buffer.Position);
            Assert.Equal(32768, buffer.StartOffset);
            Assert.Equal(32768, buffer.RemainingCapacity);

            buffer.Position += 10;
            Assert.Equal(32768, buffer.Capacity);
            Assert.Equal(32778, buffer.Position);
            Assert.Equal(32768, buffer.StartOffset);
            Assert.Equal(32758, buffer.RemainingCapacity);
        }


        [Fact]
        public void OffsetUnderRange()
        {
            var slice = new byte[65535];
            var buffer = new BufferSlice(slice, 0, slice.Length, slice.Length);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Position -= 1);
        }

        [Fact]
        public void OffsetOverRange()
        {
            var slice = new byte[65535];
            var buffer = new BufferSlice(slice, 0, slice.Length, slice.Length);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Position += slice.Length + 1);
        }

        [Fact]
        public void TestCompact()
        {
            var buffer = Encoding.ASCII.GetBytes("0123456789");
            var slice = new BufferSlice(buffer, 0, buffer.Length, buffer.Length);
            slice.Position = 5;
            slice.Compact();
            var str = Encoding.ASCII.GetString(slice.Buffer, 0, 5);
            Assert.Equal("56789", str);
        }


        [Fact]
        public void RemainingLength()
        {
            var slice = new byte[10];
            var buffer = new BufferSlice(slice, 0, slice.Length, slice.Length);
            buffer.Count = 5;
            Assert.Equal(5, buffer.RemainingLength);
        }

    }
}
