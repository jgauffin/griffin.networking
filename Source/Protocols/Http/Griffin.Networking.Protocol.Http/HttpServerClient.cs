using Griffin.Networking.Buffers;
using Griffin.Networking.Http.Implementation;
using Griffin.Networking.Http.Pipeline.Handlers;
using Griffin.Networking.Http.Protocol;
using Griffin.Networking.Servers;

namespace Griffin.Networking.Http
{
    /// <summary>
    /// Lightweight client which just parses the HTTP message and sends it along.
    /// </summary>
    public class HttpServerClient : ServerClientContext
    {
        private HttpParser _parser = new HttpParser();
        StringBufferSliceReader _reader = new StringBufferSliceReader();
        private static IBufferSliceStack _stack = new BufferSliceStack(100, 65535);

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpServerClient" /> class.
        /// </summary>
        /// <param name="readBuffer">The read buffer.</param>
        public HttpServerClient(IBufferSlice readBuffer) : base(readBuffer)
        {
        }

        /// <summary>
        /// Handle incoming bytes
        /// </summary>
        /// <param name="readBuffer">Buffer containing received bytes</param>
        /// <param name="bytesReceived">Number of bytes that was recieved (will always be set, any errors have triggered <see cref="ServerClientContext.OnDisconnect" /> instead).</param>
        /// <remarks>
        /// The default implementation will trigger the client with a <see cref="IBufferReader" /> as message. That means that
        /// you should not call the base method from your code.
        /// </remarks>
        protected override void HandleRead(IBufferSlice readBuffer, int bytesReceived)
        {
            _reader.Assign(readBuffer, bytesReceived);
            IMessage message;
            while ((message = _parser.Parse(_reader)) != null)
            {
                TriggerClientReceive(message);    
            }
        }

        protected override void OnDisconnect(System.Net.Sockets.SocketError error)
        {
            
        }

        public virtual void Send(IMessage message)
        {
            var slice = _stack.Pop();
            var stream = new SliceStream(slice);
            var serializer = new HttpHeaderSerializer();
            serializer.SerializeResponse((IResponse)message, stream);
            Send(slice, (int) stream.Length);

            slice = _stack.Pop();

        }
    }


}