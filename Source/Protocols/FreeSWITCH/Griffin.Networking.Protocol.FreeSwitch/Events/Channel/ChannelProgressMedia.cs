namespace Griffin.Networking.Protocol.FreeSwitch.Events.Channel
{
    /// <summary>
    /// A channel has started early media
    /// </summary>
    [EventName("CHANNEL_PROGRESS_MEDIA")]
    public class ChannelProgressMedia : ChannelStateEvent
    {
        public override string ToString()
        {
            return "ChannelProgressMedia" + base.ToString();
        }
    }
}