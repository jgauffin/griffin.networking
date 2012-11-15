namespace Griffin.Networking.Http.Server
{
    /// <summary>
    /// Used to control module behaviour
    /// </summary>
    public enum ModuleResult
    {
        /// <summary>
        /// Continue with the next module
        /// </summary>
        Continue,

        /// <summary>
        /// Stop processing more modules
        /// </summary>
        Stop,
    }
}