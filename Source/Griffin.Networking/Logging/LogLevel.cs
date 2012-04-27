namespace Griffin.Networking.Logging
{
    /// <summary>
    /// Log levels
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Very detailed logs used during diagnostics
        /// </summary>
        Trace,

        /// <summary>
        /// Diagnostics
        /// </summary>
        Debug,

        /// <summary>
        /// Events and similar
        /// </summary>
        Info,

        /// <summary>
        /// Something failed, but processing can continue
        /// </summary>
        Warning,

        /// <summary>
        /// Something failed, expected execution path can not succeed.
        /// </summary>
        Error
    }
}