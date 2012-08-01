namespace Griffin.Networking.JsonRpc
{
    /// <summary>
    /// When a rpc call encounters an error, the Response Object MUST contain this class
    /// </summary>
    public class RpcError
    {
        /// <summary>
        /// Gets or sets one of the standard errors (<see cref="RpcErrorCode"/>) or an application specific error code
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Gets or sets a short description of the error.
        /// </summary>
        /// <remarks>The message SHOULD be limited to a concise single sentence.</remarks>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets a primitive or structured value that contains additional information about the error.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The value of this member is defined by the Server (e.g. detailed error information, nested errors etc)
        /// </para>
        /// <para>This may be omitted.</para>
        /// </remarks>
        public object Data { get; set; }
    }
}