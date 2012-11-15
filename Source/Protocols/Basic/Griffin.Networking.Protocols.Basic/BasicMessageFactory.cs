using Griffin.Networking.Messaging;

namespace Griffin.Networking.Protocols.Basic
{
    /// <summary>
    /// Used to construct the messaging serializer/builder
    /// </summary>
    public class BasicMessageFactory : IMessageFormatterFactory
    {
        #region IMessageFormatterFactory Members

        /// <summary>
        /// Create a new serializer (used to convert messages to byte buffers)
        /// </summary>
        /// <returns><see cref="BasicMessageSerializer"/> per default</returns>
        public IMessageSerializer CreateSerializer()
        {
            return new BasicMessageSerializer();
        }

        /// <summary>
        /// Create a message builder (used to compose messages from byte buffers)
        /// </summary>
        /// <returns><see cref="BasicMessageBuilder"/> per default</returns>
        public IMessageBuilder CreateBuilder()
        {
            return new BasicMessageBuilder();
        }

        #endregion
    }
}