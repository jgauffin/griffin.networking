using Griffin.Networking.Messages;

namespace Griffin.Networking
{
    /// <summary>
    /// Pipeline used to send information to/from a channel
    /// </summary>
    /// <remarks>
    /// <para>
    /// Pipelines are used to be let different handlers process
    /// messages to/from an <see cref="IChannel"/> in an orderly fashion.
    /// </para>
    /// <para>
    /// All messages that a <see cref="IPipelineHandler"/> can't process should be sent to the next handler
    /// using the supplied <see cref="IPipelineHandlerContext"/>.
    /// </para>
    /// <para>
    /// Any failures in the pipeline should be be caught by the handler itself and sent upstream using the <see cref="PipelineFailure"/> message. It gives the
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
        /// <param name="channel">channel which will handle all down stream messages</param>
        void SetChannel(IChannel channel);

        /// <summary>
        /// Send a message from the client and downwards.
        /// </summary>
        /// <param name="message">Message to send to the channel</param>
        void SendDownstream(IPipelineMessage message);
    }
}