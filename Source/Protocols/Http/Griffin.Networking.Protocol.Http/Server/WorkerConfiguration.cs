using Griffin.Networking.Buffers;

namespace Griffin.Networking.Protocol.Http.Server
{
    /// <summary>
    /// Configuration for <see cref="HttpServerWorker"/>.
    /// </summary>
    public class WorkerConfiguration
    {
        public IModuleManager ModuleManager { get; set; }

        /// <summary>
        /// Used to store items for the entire application.
        /// </summary>
        /// <remarks>These items are shared between all requests and suers</remarks>
        public IItemStorage Application { get; set; }

        public IBufferSliceStack BufferSliceStack { get; set; }
    }
}