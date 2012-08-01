namespace Griffin.Networking.Protocol.FreeSwitch.Commands
{
    /// <summary>
    /// A reply to <see cref="ICommand"/>.
    /// </summary>
    public interface ICommandReply
    {
        /// <summary>
        /// Gets whether the command was successful
        /// </summary>
        bool IsSuccessful { get; }

        /// <summary>
        /// Gets command result body
        /// </summary>
        /// <remarks>
        /// Error message or actual result.
        /// </remarks>
        string Body { get; }
    }
}