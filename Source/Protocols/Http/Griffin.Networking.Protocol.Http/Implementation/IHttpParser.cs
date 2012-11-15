using Griffin.Networking.Buffers;
using Griffin.Networking.Http.Protocol;

namespace Griffin.Networking.Http.Implementation
{
    /// <summary>
    /// Parses HTTP messages
    /// </summary>
    public interface IHttpParser
    {
        IMessage Parse(IStringBufferReader reader);

        /// <summary>
        /// Reset parser to initial state.
        /// </summary>
        void Reset();
    }
}