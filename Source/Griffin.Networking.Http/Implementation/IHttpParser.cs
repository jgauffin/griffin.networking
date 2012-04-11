using Griffin.Networking.Buffers;
using Griffin.Networking.Http.Protocol;

namespace Griffin.Networking.Http.Implementation
{
    public interface IHttpParser
    {
        IMessage Parse(BufferSlice bufferSlice);

        /// <summary>
        /// Reset parser to initial state.
        /// </summary>
        void Reset();
    }
}