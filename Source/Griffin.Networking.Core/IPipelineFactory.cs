namespace Griffin.Networking
{
    /// <summary>
    /// Used to produce the pipeline that is used by a connection
    /// </summary>
    public interface IPipelineFactory
    {
        /// <summary>
        /// Create a pipeline for a channel
        /// </summary>
        /// <returns>Created pipeline</returns>
        IPipeline Build();
    }
}