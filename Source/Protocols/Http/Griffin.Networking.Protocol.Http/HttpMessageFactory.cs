using Griffin.Networking.Messaging;

namespace Griffin.Networking.Protocol.Http
{
    /// <summary>
    /// Used to convert byte[] arrays to/from HTTP messages.
    /// </summary>
    public class HttpMessageFactory : IMessageFormatterFactory
    {
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
            return new HttpMessageBuilder();
        }

        #endregion
    }
}