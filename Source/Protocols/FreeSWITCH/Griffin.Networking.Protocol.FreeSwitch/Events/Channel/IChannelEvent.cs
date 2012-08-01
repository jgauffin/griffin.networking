namespace Griffin.Networking.Protocol.FreeSwitch.Events.Channel
{
    /// <summary>
    /// Gets event for a channel
    /// </summary>
    public interface IChannelEvent : IFreeSwitchEvent
    {
        /// <summary>
        /// Gets id of the channel that the event is for.
        /// </summary>
        UniqueId ChannelId { get; }
    }
}