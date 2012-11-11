namespace Griffin.Networking.Pipelines
{
    /// <summary>
    /// A handler used to process everything sent from the client down to the channel
    /// </summary>
    public interface IDownstreamHandler : IPipelineHandler
    {
        /// <summary>
        /// Process message
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        /// <remarks>
        /// Should always call either <see cref="IPipelineHandlerContext.SendDownstream"/> or <see cref="IPipelineHandlerContext.SendUpstream"/>
        /// unless the handler really wants to stop the processing.
        /// </remarks>
        void HandleDownstream(IPipelineHandlerContext context, IPipelineMessage message);
    }
}