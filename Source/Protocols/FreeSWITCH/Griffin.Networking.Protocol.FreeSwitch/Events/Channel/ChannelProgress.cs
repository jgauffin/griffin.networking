namespace Griffin.Networking.Protocol.FreeSwitch.Events.Channel
{
    /// <summary>
    /// A channel have started ringing
    /// </summary>
    [EventName("CHANNEL_PROGRESS")]
    public class ChannelProgress : ChannelStateEvent
    {
        public override string ToString()
        {
            return "ChannelProgress" + base.ToString();
        }
    }
}