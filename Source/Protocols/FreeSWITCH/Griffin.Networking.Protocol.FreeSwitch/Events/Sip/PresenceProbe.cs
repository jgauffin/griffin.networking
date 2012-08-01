namespace Griffin.Networking.Protocol.FreeSwitch.Events.Sip
{
    /// <summary>
    /// 
    /// </summary>
    /// <example>
    /// proto: sip
    /// login: sip%3Amod_sofia%4010.0.1.250%3A5060
    /// from: 1009%4010.0.1.250
    /// status: Available
    /// event_type: presence
    /// event_subtype: probe
    /// proto-specific-event-name: dialog
    /// Event-Name: PRESENCE_PROBE
    /// Core-UUID: 2130a7d1-c1f7-44cd-8fae-8ed5946f3cec
    /// FreeSWITCH-Hostname: localhost.localdomain
    /// FreeSWITCH-IPv4: 10.0.1.250
    /// FreeSWITCH-IPv6: 127.0.0.1
    /// Event-Date-Local: 2007-12-16%2022%3A31%3A16
    /// Event-Date-GMT: Mon,%2017%20Dec%202007%2004%3A31%3A16%20GMT
    /// Event-Date-timestamp: 1197865876565022
    /// Event-Calling-File: sofia_presence.c
    /// Event-Calling-Function: sofia_presence_sub_reg_callback
    /// Event-Calling-Line-Number: 484
    /// </example>
    [EventName("PRESENCE_PROBE")]
    public class PresenceProbe : SipEvent
    {
        /// <summary>
        /// Gets or sets user status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets login module used.
        /// </summary>
        public string Login { get; set; }

        public override bool ParseParameter(string name, string value)
        {
            switch (name)
            {
                case "status":
                    Status = value;
                    break;
                case "login":
                    Login = value;
                    break;
            }
            return true;
        }
    }
}