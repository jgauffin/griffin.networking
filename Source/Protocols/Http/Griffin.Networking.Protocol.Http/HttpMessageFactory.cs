using Griffin.Networking.Buffers;
using Griffin.Networking.Messaging;

namespace Griffin.Networking.Protocol.Http
{
    /// <summary>
    /// Used to convert byte[] arrays to/from HTTP messages.
    /// </summary>
    public class HttpMessageFactory : IMessageFormatterFactory
    {
        private readonly IBufferSliceStack _stack;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMessageFactory"/> class.
        /// </summary>
        /// <param name="stack">Used to provide <c>byte[]</c> buffers to the workers..</param>
        public HttpMessageFactory(IBufferSliceStack stack)
        {
            _stack = stack;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMessageFactory"/> class.
        /// </summary>
        public HttpMessageFactory()
        {
            _stack = new BufferSliceStack(100, 65535);
        }

        #region IMessageFormatterFactory Members

        /// <summary>
        /// Create a new serializer (used to convert messages to byte buffers)
        /// </summary>
        /// <returns>Created serializer</returns>
        public IMessageSerializer CreateSerializer()
        {
            return new HttpMessageSerializer();
        }

        /// <summary>
        /// Create a message builder (used to compose messages from byte buffers)
        /// </summary>
        /// <returns></returns>
        public IMessageBuilder CreateBuilder()
        {
            return new HttpMessageBuilder(_stack);
        }

        #endregion
    }
}