using System;
using System.Text;
using Griffin.Networking.Buffers;
using Xunit;

namespace Griffin.Networking.Tests.Buffers
{
    public class SliceStreamTests
    {
        [Fact]
        public void Test()
        {
            var slice = new BufferSlice(65535);
            var stream = new SliceStream(slice);

            Assert.Equal(0, stream.Position);
            Assert.Equal(0, stream.Length);
            Assert.Equal(65535, ((IBufferWrapper) stream).Capacity);
        }

        [Fact]
        public void Write_Single()
        {
            var slice = new BufferSlice(65535);
            var stream = new SliceStream(slice);

            var mammasBullar = Encoding.UTF8.GetBytes("Mammas bullar smakar godast.");
            stream.Write(mammasBullar, 0, mammasBullar.Length);

            Assert.Equal(mammasBullar.Length, stream.Position);
            Assert.Equal(mammasBullar.Length, stream.Length);

            // must be able to write after the last byte.
            stream.Position = mammasBullar.Length;
        }

        [Fact]
        public void Write_WrongSize()
        {
            var slice = new BufferSlice(65535);
            var stream = new SliceStream(slice);

            var mammasBullar = Encoding.UTF8.GetBytes("Mammas bullar smakar godast.");
            Assert.Throws<ArgumentOutOfRangeException>(() => stream.Write(mammasBullar, 5, mammasBullar.Length));
        }

        [Fact]
        public void Write_WrongIndex()
        {
            var slice = new BufferSlice(65535);
            var stream = new SliceStream(slice);

            var mammasBullar = Encoding.UTF8.GetBytes("Mammas bullar smakar godast.");
            Assert.Throws<ArgumentOutOfRangeException>(() => stream.Write(mammasBullar, 50, 3));
        }

        [Fact]
        public void Write_TooSmallInternalBuffer()
        {
            var slice = new BufferSlice(5);
            var stream = new SliceStream(slice);

            var mammasBullar = Encoding.UTF8.GetBytes("Mammas bullar smakar godast.");
            Assert.Throws<ArgumentOutOfRangeException>(() => stream.Write(mammasBullar, 0, 6));
        }

        [Fact]
        public void Write_WrongOffset()
        {
            var slice = new BufferSlice(5);
            var stream = new SliceStream(slice);

            var mammasBullar = Encoding.UTF8.GetBytes("Mammas bullar smakar godast.");
            Assert.Throws<ArgumentOutOfRangeException>(() => stream.Write(mammasBullar, -1, 1));
        }


        [Fact]
        public void Write_Two()
        {
            var slice = new BufferSlice(65535);
            var stream = new SliceStream(slice);

            var mammasBullar = Encoding.UTF8.GetBytes("Mammas bullar smakar godast.");
            stream.Write(mammasBullar, 0, mammasBullar.Length);
            stream.Write(mammasBullar, 0, mammasBullar.Length);

            Assert.Equal(mammasBullar.Length*2, stream.Position);
            Assert.Equal(mammasBullar.Length*2, stream.Length);

            // must be able to write after the last byte.
            stream.Position = mammasBullar.Length*2;
        }

        [Fact]
        public void Write_Overwrite()
        {
            var slice = new BufferSlice(65535);
            var stream = new SliceStream(slice);

            var mammasBullar = Encoding.UTF8.GetBytes("Mammas bullar smakar godast.");
            stream.Write(mammasBullar, 0, mammasBullar.Length);
            stream.Position = 0;
            stream.Write(mammasBullar, 0, mammasBullar.Length);

            Assert.Equal(mammasBullar.Length, stream.Position);
            Assert.Equal(mammasBullar.Length, stream.Length);

            // must be able to write after the last byte.
            stream.Position = mammasBullar.Length;
        }


        [Fact]
        public void Write_ThenMovePositionOutOfRange()
        {
            var slice = new BufferSlice(65535);
            var stream = new SliceStream(slice);

            var mammasBullar = Encoding.UTF8.GetBytes("Mammas bullar smakar godast.");
            stream.Write(mammasBullar, 0, mammasBullar.Length);


            Assert.Throws<ArgumentOutOfRangeException>(() => stream.Position = mammasBullar.Length + 1);
            Assert.Throws<ArgumentOutOfRangeException>(() => stream.Position = -1);
        }

        [Fact]
        public void Position_SetGet()
        {
            var slice = new BufferSlice(65535);
            var stream = new SliceStream(slice);
            var mammasBullar = Encoding.UTF8.GetBytes("Mammas bullar smakar godast.");
            stream.Write(mammasBullar, 0, mammasBullar.Length);

            stream.Position = 2;
            Assert.Equal(2, stream.Position);
        }

        [Fact]
        public void Read_OneTime()
        {
            var slice = new BufferSlice(65535);
            var stream = new SliceStream(slice);
            var mammasBullar = Encoding.UTF8.GetBytes("Mammas bullar smakar godast.");
            stream.Write(mammasBullar, 0, mammasBullar.Length);

            var buffer = new byte[10];
            stream.Position = 0;
            stream.Read(buffer, 0, 6);
            
            Assert.Equal("Mammas", Encoding.UTF8.GetString(buffer, 0, 6));
        }

        [Fact]
        public void Read_TwoTimes()
        {
            var slice = new BufferSlice(65535);
            var stream = new SliceStream(slice);
            var mammasBullar = Encoding.UTF8.GetBytes("Mammas bullar smakar godast.");
            stream.Write(mammasBullar, 0, mammasBullar.Length);

            var buffer = new byte[10];
            stream.Position = 0;
            stream.Read(buffer, 0, 6);
            var buffer2 = new byte[10];
            stream.Read(buffer2, 0, 7);

            Assert.Equal("Mammas", Encoding.UTF8.GetString(buffer, 0, 6));
            Assert.Equal(" bullar", Encoding.UTF8.GetString(buffer2, 0, 7));
        }

    }
}
