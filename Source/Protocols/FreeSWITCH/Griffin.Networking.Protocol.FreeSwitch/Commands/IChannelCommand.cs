namespace Griffin.Networking.Protocol.FreeSwitch.Commands
{
    public interface IChannelCommand : ICommand
    {
        /// <summary>
        /// Gets channel id
        /// </summary>
        UniqueId ChannelId { get; }
    }
}