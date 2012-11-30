using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Griffin.Networking.Protocol.Http.Implementation;
using Xunit;

namespace Griffin.Networking.Http.Tests.Implementation
{
    public class ByteRangeStreamTests
    {
        [Fact]
        public void ReadOneRangePartially()
        {
            var stream = CreateTextStream();
            var ranges = new RangeCollection();
            ranges.Parse("bytes=0-100", (int)stream.Length);
            byte[] target = new byte[100];

            var stream2 = new ByteRangeStream(ranges, stream);
            stream2.Read(target, 0, 50);
            stream2.Read(target, 0, 50);
        }

        private static MemoryStream CreateTextStream()
        {
            var sb = new StringBuilder();
            for (int a = 0; a < 200; a = a + 3)
            {
                sb.AppendFormat(" {0,2:00}", a+3);
            }
            var stringBuffer = Encoding.ASCII.GetBytes(sb.ToString());

            var stream = new MemoryStream(stringBuffer, 0, stringBuffer.Length);
            return stream;
        }
    }
}
