using System.Text;
using Griffin.Networking.Buffers;
using Xunit;

namespace Griffin.Networking.Tests.Buffers
{
    public class BufferWorkerTests
    {
        [Fact]
        public void Init()
        {
            var buffer = Encoding.ASCII.GetBytes("0123456789");
            var worker = new BufferWriter();
            worker.Assign(new BufferSlice(buffer, 0, buffer.Length));


        }

        [Fact]
        public void ForwardAndRead()
        {
            var buffer = Encoding.ASCII.GetBytes("0123456789");
            var worker = new BufferWriter();
            worker.Assign(new BufferSlice(buffer, 0, buffer.Length));

            worker.Forward(4);
            Assert.Equal('4', (char)worker.Buffer[worker.Position]);

        }

        [Fact]
        public void Writes()
        {
            var buffer = Encoding.ASCII.GetBytes("0123456789");
            var worker = new BufferWriter();
            worker.Assign(new BufferSlice(new byte[65535], 0, 65535));

            worker.Write(buffer, 0, 5);

            Assert.Equal('4', (char)worker.Buffer[worker.Position]);

        }
    }
}
