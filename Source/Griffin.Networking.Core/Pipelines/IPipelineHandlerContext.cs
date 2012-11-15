namespace Griffin.Networking.Pipelines
{
    /// <summary>
    /// Context assigned to each channel to be able to continue down the chain or to change direction in the pipe
    /// </summary>
    /// <remarks>
    /// It depends of the type of channel how the processing is done. A <see cref="SendDownstream"/> will let the
    /// processing continue down the pipe when calling <see cref="SendUpstream"/> while it moves the message to the beginning
    /// of the pipe if calling <see cref="IDownstreamHandler"/> (to let all up stream handlers have a chance to process the message).
    /// </remarks>
    public interface IPipelineHandlerContext
    {
        /// <summary>
        /// Send message up towards the client
        /// </summary>
        /// <param name="message">Message to process</param>
        void SendUpstream(IPipelineMessage message);

        /// <summary>
        /// Sned the message down towards the channel
        /// </summary>
        /// <param name="message">Message to process</param>
        void SendDownstream(IPipelineMessage message);
    }
}