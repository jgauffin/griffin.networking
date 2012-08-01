namespace Griffin.Networking.Protocol.FreeSwitch.Events.Sip
{
    [EventName("PRESENCE_IN")]
    public class PresenceIn : Presence
    {
        public override string ToString()
        {
            return "In." + base.ToString();
        }
    }
}