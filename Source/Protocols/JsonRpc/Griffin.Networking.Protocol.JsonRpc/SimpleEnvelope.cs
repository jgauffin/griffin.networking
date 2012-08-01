namespace Griffin.Networking.JsonRpc
{
    /// <summary>
    ///   Wraps the transferred JSON request.
    /// </summary>
    public class SimpleHeader
    {
        /// <summary>
        ///   Gets or sets body length
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        ///   Gets or sets version number
        /// </summary>
        public byte Version { get; set; }
    }
}