namespace Griffin.Networking.Protocol.FreeSwitch.Events.Sip
{
    [EventName("PRESENCE_OUT")]
    public class PresenceOut : Presence
    {
        public override string ToString()
        {
            return "Out." + base.ToString();
        }
    }
}