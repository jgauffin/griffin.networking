using Griffin.Networking.Protocol.FreeSwitch.Commands;

namespace Griffin.Networking.Protocol.FreeSwitch.Net.Messages
{
    /// <summary>
    /// Send a FreeSWITCH command down the pipeline
    /// </summary>
    /// <remarks>
    /// A <see cref="CommandReply"/> will be sent upstream when the command have been handled.
    /// </remarks>
    public class SendCommandMessage : IPipelineMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendCommandMessage"/> class.
        /// </summary>
        /// <param name="command">Command to send.</param>
        public SendCommandMessage(ICommand command)
        {
            Command = command;
        }

        /// <summary>
        /// Gets command to send.
        /// </summary>
        public ICommand Command { get; private set; }
    }
}