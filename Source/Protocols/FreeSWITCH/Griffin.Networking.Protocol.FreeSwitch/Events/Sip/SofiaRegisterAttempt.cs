using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.Networking.Protocol.FreeSwitch.Events.Sip
{
    /// <summary>
    /// Tries to register / unregister in the module
    /// </summary>
    [EventName("SOFIA::REGISTER_ATTEMPT", IsCustom = true)]
    public class SofiaRegisterAttempt : EventBase
    {
    }

    /*
     * Event-Subclass: sofia%3A%3Aregister_attempt
Event-Name: CUSTOM
Core-UUID: 5463eedc-14dc-4634-bcae-e8240ce37f2a
FreeSWITCH-Hostname: jobbpc
FreeSWITCH-Switchname: jobbpc
FreeSWITCH-IPv4: 192.168.1.65
FreeSWITCH-IPv6: 2001%3A0%3A5ef5%3A79fd%3A6f%3A1ff6%3Aae27%3A2e35
Event-Date-Local: 2012-06-26%2022%3A26%3A12
Event-Date-GMT: Tue,%2026%20Jun%202012%2020%3A26%3A12%20GMT
Event-Date-Timestamp: 1340742372002759
Event-Calling-File: sofia_reg.c
Event-Calling-Function: sofia_reg_handle_register
Event-Calling-Line-Number: 1228
Event-Sequence: 3553
profile-name: internal
from-user: 1000
from-host: 192.168.1.65
contact: %22Jonas%22%20%3Csip%3A1000%4081.216.209.202%3A35000%3Brinstance%3Dce0375219b132ee9%3E
rpid: unknown
status: Registered(UDP)
expires: 0
to-user: 1000
to-host: internal
network-ip: 192.168.1.65
network-port: 35000
username: 1000
realm: internal
user-agent: X-Lite%204%20release%204.1%20stamp%2063214*/
}
