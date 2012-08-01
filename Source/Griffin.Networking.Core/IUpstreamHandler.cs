namespace Griffin.Networking
{
    /// <summary>
    /// Used to process messages that are sent from the channel up towards the client
    /// </summary>
    public interface IUpstreamHandler : IPipelineHandler
    {
        /// <summary>
        /// Handle an message
        /// </summary>
        /// <param name="context">Context unique for this handler instance</param>
        /// <param name="message">Message to process</param>
        /// <remarks>
        /// All messages that can't be handled MUST be send up the chain using <see cref="IPipelineHandlerContext.SendUpstream"/>.
        /// </remarks>
        void HandleUpstream(IPipelineHandlerContext context, IPipelineMessage message);
    }
}