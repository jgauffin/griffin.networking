namespace Griffin.Networking.Messaging
{
    /// <summary>
    /// The following factory are used to provide the classes that are used to convert your .NET POCOS to something
    /// that can be transported over a socket and vice versa.
    /// </summary>
    public interface IMessageFormatterFactory
    {
        /// <summary>
        /// Create a new serializer (used to convert messages to byte buffers)
        /// </summary>
        /// <returns>Created serializer</returns>
        IMessageSerializer CreateSerializer();

        /// <summary>
        /// Create a message builder (used to compose messages from byte buffers)
        /// </summary>
        /// <returns></returns>
        IMessageBuilder CreateBuilder();
    }
}