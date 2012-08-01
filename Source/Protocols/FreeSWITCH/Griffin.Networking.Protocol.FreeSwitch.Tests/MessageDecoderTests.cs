using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Griffin.Networking.Buffers;
using Griffin.Networking.Messages;
using Griffin.Networking.Protocol.FreeSwitch.Net.Handlers;
using Griffin.Networking.Protocol.FreeSwitch.Net.Messages;
using Xunit;

namespace Griffin.Networking.Protocol.FreeSwitch.Tests
{
    public class MessageDecoderTests
    {
        private Received _msg;
        private MessageDecoder _decoder;
        private MyCtx _context;
        private const string Auth = "Content-Type: auth/request\n\n";

        private const string Messages =
            "Content-Type: event\nContent-Length:10\n\n1234567890\n\nContent-Type:event\nContent-Length:0\n\n";
        private const string Messages2 =
            "Content-Type:event\nContent-Length:0\n\nContent-Type: event\nContent-Length:10\n\n1234567890\n\n";

        private const string Partial1 =
            "Content-Type: event\nContent-Length:10\n\n12345";
        private const string Partial2 =
            "67890\n\nContent-Type:event\nContent-Length:0\n\n";


        private void Build(byte[] buffer)
        {
            _msg = new Received(new IPEndPoint(IPAddress.Any, 90), null, new BufferSlice(buffer, 0, buffer.Length, buffer.Length));
            if (_decoder == null)
            {
                _decoder = new MessageDecoder();
                _context = new MyCtx();
            }
        }

        [Fact]
        public void TestAuthRequest()
        {
            var body = Encoding.ASCII.GetBytes("Content-Type: auth/request\n\n");
            Build(body);

            _decoder.HandleUpstream(_context, _msg);
            if (_context.Upstream.First() is PipelineFailure)
                throw new TargetInvocationException(((PipelineFailure)_context.Upstream.First()).Exception);

            Assert.IsType<ReceivedMessage>(_context.Upstream.First());
            var msG = (ReceivedMessage)_context.Upstream.First();
            Assert.Equal("auth/request", msG.Message.Headers["Content-Type"]);
        }

        [Fact]
        public void TestSimpleMessage()
        {
            var body = "Content-Type: event\nEvent-Name: heartbeat\nContent-Length: 0\n\n";
            var buffer = Encoding.ASCII.GetBytes(body);
            Build(buffer);

            _decoder.HandleUpstream(_context, _msg);
            if (_context.Upstream.First() is PipelineFailure)
                throw new TargetInvocationException(((PipelineFailure)_context.Upstream.First()).Exception);

            Assert.IsType<ReceivedMessage>(_context.Upstream.First());
            var msG = (ReceivedMessage)_context.Upstream.First();
            Assert.Equal("heartbeat", msG.Message.Headers["Event-Name"]);
        }

        [Fact]
        public void TestMessage()
        {
            var buffer = Assembly.GetExecutingAssembly().ReadAsBytes("MessageQuery.txt");
            Build(buffer);

            _decoder.HandleUpstream(_context, _msg);
            if (_context.Upstream.First() is PipelineFailure)
                throw new TargetInvocationException(((PipelineFailure)_context.Upstream.First()).Exception);

            Assert.IsType<ReceivedMessage>(_context.Upstream.First());
            var msG = (ReceivedMessage)_context.Upstream.First();
            Assert.Equal("text/event-plain", msG.Message.Headers["Content-Type"]);
            Assert.Equal("630", msG.Message.Headers["Content-Length"]);
            Assert.Equal(630, msG.Message.Body.Length);
        }

        [Fact]
        public void TestTwoHalves()
        {
            var buffer = Assembly.GetExecutingAssembly().ReadAsBytes("MsgPart1.txt");
            Build(buffer);

            _decoder.HandleUpstream(_context, _msg);
            Assert.Equal(0, _context.Upstream.Count);
            Assert.Equal(52, _msg.BufferSlice.Position);

            buffer = Assembly.GetExecutingAssembly().ReadAsBytes("MsgPart2.txt");
            Build(buffer);

            _decoder.HandleUpstream(_context, _msg);

            if (_context.Upstream.First() is PipelineFailure)
                throw new TargetInvocationException(((PipelineFailure)_context.Upstream.First()).Exception);

            Assert.IsType<ReceivedMessage>(_context.Upstream.First());
            var msG = (ReceivedMessage)_context.Upstream.First();
            Assert.Equal("text/event-plain", msG.Message.Headers["Content-Type"]);
            Assert.Equal("651", msG.Message.Headers["Content-Length"]);
            Assert.Equal(651, msG.Message.Body.Length);
        }

        [Fact]
        public void TestTwoMessagesWithCompact()
        {
            var buffer = Assembly.GetExecutingAssembly().ReadAsBytes("TwoMessages.txt");
            Build(buffer);

            _decoder.HandleUpstream(_context, _msg);
            Assert.Equal(1, _context.Upstream.Count);
            Assert.Equal(682, _msg.BufferSlice.Position);
            Assert.IsType<ReceivedMessage>(_context.Upstream.First());
            var msG = (ReceivedMessage)_context.Upstream.First();
            Assert.Equal("text/event-plain", msG.Message.Headers["Content-Type"]);
            Assert.Equal("630", msG.Message.Headers["Content-Length"]);
            Assert.Equal(630, msG.Message.Body.Length);
            _context.Upstream.Clear();

            _msg.BufferSlice.Compact();
            Build(buffer);

            _decoder.HandleUpstream(_context, _msg);

            if (_context.Upstream.First() is PipelineFailure)
                throw new TargetInvocationException(((PipelineFailure)_context.Upstream.First()).Exception);

            Assert.IsType<ReceivedMessage>(_context.Upstream.First());
            msG = (ReceivedMessage)_context.Upstream.First();
            Assert.Equal("text/event-plain", msG.Message.Headers["Content-Type"]);
            Assert.Equal("754", msG.Message.Headers["Content-Length"]);
            Assert.Equal(754, msG.Message.Body.Length);
            Assert.Equal(806, _msg.BufferSlice.Position);
        }

        [Fact]
        public void TestTwoMessagesDirectly()
        {
            var buffer = Assembly.GetExecutingAssembly().ReadAsBytes("TwoMessages.txt");
            Build(buffer);

            _decoder.HandleUpstream(_context, _msg);
            Assert.Equal(1, _context.Upstream.Count);
            Assert.Equal(682, _msg.BufferSlice.Position);
            Assert.IsType<ReceivedMessage>(_context.Upstream.First());
            var msG = (ReceivedMessage)_context.Upstream.First();
            Assert.Equal("text/event-plain", msG.Message.Headers["Content-Type"]);
            Assert.Equal("630", msG.Message.Headers["Content-Length"]);
            Assert.Equal(630, msG.Message.Body.Length);
            _context.Upstream.Clear();

            _decoder.HandleUpstream(_context, _msg);

            if (_context.Upstream.First() is PipelineFailure)
                throw new TargetInvocationException(((PipelineFailure)_context.Upstream.First()).Exception);

            Assert.IsType<ReceivedMessage>(_context.Upstream.First());
            msG = (ReceivedMessage)_context.Upstream.First();
            Assert.Equal("text/event-plain", msG.Message.Headers["Content-Type"]);
            Assert.Equal("754", msG.Message.Headers["Content-Length"]);
            Assert.Equal(754, msG.Message.Body.Length);
            Assert.Equal(1488, _msg.BufferSlice.Position);
        }

    }
}
