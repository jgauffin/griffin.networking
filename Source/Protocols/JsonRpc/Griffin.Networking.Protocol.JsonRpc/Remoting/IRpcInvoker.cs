namespace Griffin.Networking.JsonRpc.Remoting
{
    /// <summary>
    /// Invoke JSON RPC service methods.
    /// </summary>
    public interface IRpcInvoker
    {
        /// <summary>
        /// Invoke the RPC request.
        /// </summary>
        /// <param name="request">RPC request</param>
        /// <returns>Response to send back</returns>
        ResponseBase Invoke(Request request);
    }
}