namespace Griffin.Networking.Protocol.FreeSwitch.Events.Channel
{
    [EventName("CHANNEL_ANSWER")]
    public class ChannelAnswer : ChannelStateEvent
    {
        public override string ToString()
        {
            return "ChannelAnswer." + base.ToString();
        }
    }
}