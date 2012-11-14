namespace Griffin.Networking.JsonRpc
{
    /// <summary>
    /// Defined RPC error codes.
    /// </summary>
    /// <remarks>
    ///-32000 to -32099	Server error	Reserved for implementation-defined server-errors.
    /// </remarks>
    public static class RpcErrorCode
    {
        /// <summary>
        /// Invalid JSON was received by the server. 
        /// </summary>
        /// <remarks>An error occurred on the server while parsing the JSON text</remarks>
        public const int ParserError = -32700;

        /// <summary>
        /// he JSON sent is not a valid Request object.
        /// </summary>
        public const int InvalidRequest = -32600;

        /// <summary>
        /// The method does not exist / is not available.
        /// </summary>
        public const int MethodNotFound = -32601;

        /// <summary>
        /// Invalid method parameter(s).
        /// </summary>
        public const int InvalidParameters = -32602;

        /// <summary>
        /// Internal JSON-RPC error.
        /// </summary>
        public const int InternalError = -32603;
    }
}