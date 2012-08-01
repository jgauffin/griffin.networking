namespace Griffin.Networking.Protocol.FreeSwitch.Events.Channel
{
    [EventName("CHANNEL_ORIGINATE")]
    public class ChannelOriginate : ChannelStateEvent
    {
        public override string ToString()
        {
            return "ChannelOriginate." + base.ToString();
        }
    }
}