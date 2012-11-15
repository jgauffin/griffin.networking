using Griffin.Networking.Http.Protocol;

namespace Griffin.Networking.Http.Server
{
    /// <summary>
    /// Request context information
    /// </summary>
    public interface IRequestContext
    {
        /// <summary>
        /// Incoming request
        /// </summary>
        IRequest Request { get; }

        /// <summary>
        /// Response to send back
        /// </summary>
        IResponse Response { get; }

        /// <summary>
        /// Can be used to store items through this request
        /// </summary>
        IItemStorage Items { get; }

        /// <summary>
        /// Used to store items for the entire application.
        /// </summary>
        IItemStorage Application { get; }

        /// <summary>
        /// USed to store items for the current session (if a session has been started)
        /// </summary>
        /// <remarks>Will be null if a session has not been started.</remarks>
        IItemStorage Session { get; }
    }
}