using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Griffin.Networking.Protocol.Http.Implementation;
using Xunit;

namespace Griffin.Networking.Http.Tests.Implementation
{
    public class RangeTests
    {
        [Fact]
        public void Constructor_FirstHalf()
        {
            var range = new Range("0-499", 10000);
            Assert.Equal(500, range.Count);
            Assert.Equal(0, range.Position);
            Assert.Equal(499, range.EndPosition);
        }

        [Fact]
        public void Constructor_SecondHalf()
        {
            var range = new Range("500-999", 10000);
            Assert.Equal(500, range.Count);
            Assert.Equal(500, range.Position);
            Assert.Equal(999, range.EndPosition);
        }

        [Fact]
        public void Constructor_FinalBytes()
        {
            var range = new Range("-500", 10000);
            Assert.Equal(500, range.Count);
            Assert.Equal(9500, range.Position);
            Assert.Equal(9999, range.EndPosition);
        }

        [Fact]
        public void Constructor_FinalBytes2()
        {
            var range = new Range("9500-", 10000);
            Assert.Equal(500, range.Count);
            Assert.Equal(9500, range.Position);
            Assert.Equal(9999, range.EndPosition);
        }
    

        [Fact]
        public void OneRange_TwoReads()
        {
            var range = new Range("0-49", 100);
            var stream = new MemoryStream();
            var bytes1 = Encoding.ASCII.GetBytes("1".PadLeft(50));
            var bytes2 = Encoding.ASCII.GetBytes("2".PadLeft(50));
            stream.Write(bytes1, 0, bytes1.Length);
            stream.Write(bytes2, 0, bytes2.Length);

            var sr = new StreamReader(stream);
            stream.Position = 0;
            Console.WriteLine(sr.ReadToEnd());
            ;
            stream.Position = 0;

            var buffer = new byte[100];
            Assert.Equal(25, range.Read(stream, buffer, 0, 25));
            Assert.Equal(25, range.Read(stream, buffer, 25, 50));
            Assert.Equal("1".PadLeft(50), Encoding.ASCII.GetString(buffer, 0, 50));


        }
    }
}
