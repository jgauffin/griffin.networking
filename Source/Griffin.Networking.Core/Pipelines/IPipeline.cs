using Griffin.Networking.Pipelines.Messages;

namespace Griffin.Networking.Pipelines
{
    /// <summary>
    /// Pipeline used to send information to/from a channel
    /// </summary>
    /// <remarks>
    /// <para>
    /// All messages that a <see cref="IPipelineHandler"/> can't process should be sent to the next handler
    /// using the supplied <see cref="PipelineFailure"/>.
    /// </para>
    /// <para>
    /// Any failures in the pipeline should be be caught by the handler itself and sent upstream using the <see cref="IPipelineHandlerContext"/> message. It gives the
    /// client a chance to decide which action to be taken.
    /// </para>
    /// </remarks>
    public interface IPipeline
    {
        /// <summary>
        /// Send something from the channel to all handlers.
        /// </summary>
        /// <param name="message">Message to send to the client</param>
        void SendUpstream(IPipelineMessage message);

        /// <summary>
        /// Set down stream end point
        /// </summary>
        /// <param name="handler"> </param>
        void SetChannel(IDownstreamHandler handler);

        /// <summary>
        /// Send a message from the client and downwards.
        /// </summary>
        /// <param name="message">Message to send to the channel</param>
        void SendDownstream(IPipelineMessage message);
    }
}