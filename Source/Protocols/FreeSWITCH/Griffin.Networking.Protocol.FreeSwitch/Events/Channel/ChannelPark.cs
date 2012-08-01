namespace Griffin.Networking.Protocol.FreeSwitch.Events.Channel
{
    [EventName("CHANNEL_PARK")]
    public class ChannelPark : ChannelStateEvent
    {
        public override string ToString()
        {
            return "ChannelPark." + base.ToString();
        }
    }
}