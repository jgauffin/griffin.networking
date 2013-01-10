using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Griffin.Networking.Buffers;
using Griffin.Networking.Protocol.Http.Implementation;
using NSubstitute;
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
            ranges.Parse("bytes=0-99", (int)stream.Length);
            byte[] target = new byte[100];

            var stream2 = new ByteRangeStream(ranges, stream);
            stream2.Read(target, 0, 50);
            stream2.Read(target, 50, 50);

            Assert.Equal(GetString().Substring(0,100), Encoding.ASCII.GetString(target, 0, 100));
        }

        public void ViaSocketWriter()
        {
            var ranges = new RangeCollection();

            var fileStream = new FileStream(@"C:\Users\jgauffin\Downloads\AspNetMVC3ToolsUpdateSetup.exe", FileMode.Open,
                                                FileAccess.Read, FileShare.ReadWrite);
            ranges.Parse("bytes=0-50000", (int)fileStream.Length);
            var stream = new ByteRangeStream(ranges, fileStream);
            var job = new StreamSocketWriterJob(stream);

            var buffer = new byte[65535];
            var args = Substitute.For<SocketAsyncEventArgs>();
            args.UserToken.Returns(buffer);
            job.Write(args);
            

            while (true)
            {
                if (job.WriteCompleted(5000))
                {
                    job.Dispose();
                    break;
                }

                job.Write(args);    
            }
            
        }

        private static MemoryStream CreateTextStream()
        {
            var sb = GetString();
            var stringBuffer = Encoding.ASCII.GetBytes(sb.ToString());

            var stream = new MemoryStream(stringBuffer, 0, stringBuffer.Length);
            return stream;
        }

        private static String GetString()
        {
            var sb = new StringBuilder();
            for (int a = 0; a < 200; a = a + 3)
            {
                sb.AppendFormat(" {0,2:00}", a + 3);
            }
            return sb.ToString();
        }
    }
}
