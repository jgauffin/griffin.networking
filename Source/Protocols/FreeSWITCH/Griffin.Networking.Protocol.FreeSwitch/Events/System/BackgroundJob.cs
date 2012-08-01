using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.Networking.Protocol.FreeSwitch.Events.System
{
    /// <summary>
    /// Result for a background job.
    /// </summary>
    [EventName("BACKGROUND_JOB")]
    public class BackgroundJob : EventBase
    {
        /// <summary>
        /// Gets ID of the bg api job
        /// </summary>
        public string JobUid { get; set; }

        /// <summary>
        /// Gets command which was executed
        /// </summary>
        public string CommandName { get; set; }

        /// <summary>
        /// Gets arguments for the command
        /// </summary>
        public string CommandArguments { get; set; }

        /// <summary>
        /// Gets the actual command result.
        /// </summary>
        public string CommandResult { get; set; }

        /// <summary>
        /// Parse a parameter from FreeSWITCH
        /// </summary>
        /// <param name="name">Parameter name as defined by FS</param>
        /// <param name="value">Parameter value</param>
        /// <returns>
        /// true if parsed sucessfully; otherwise false.
        /// </returns>
        public override bool ParseParameter(string name, string value)
        {
            switch (name)
            {
                case "job-uuid":
                    JobUid = value;
                    break;
                case "job-command":
                    CommandName = value;
                    break;
                case "job-command-arg":
                    CommandArguments = value;
                    break;
                case "__content__":
                    CommandResult = value.TrimEnd('\n');
                    break;
                default:
                    return base.ParseParameter(name, value);
            }

            return true;
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return CommandName + "(" + CommandArguments + ") = '" + CommandResult + "'\r\n\t" + base.ToString();
        }
    }

    /*Event-Name: BACKGROUND_JOB
Core-UUID: 5463eedc-14dc-4634-bcae-e8240ce37f2a
FreeSWITCH-Hostname: jobbpc
FreeSWITCH-Switchname: jobbpc
FreeSWITCH-IPv4: 192.168.1.65
FreeSWITCH-IPv6: 2001%3A0%3A5ef5%3A79fd%3A6f%3A1ff6%3Aae27%3A2e35
Event-Date-Local: 2012-06-26%2020%3A59%3A54
Event-Date-GMT: Tue,%2026%20Jun%202012%2018%3A59%3A54%20GMT
Event-Date-Timestamp: 1340737194926647
Event-Calling-File: mod_event_socket.c
Event-Calling-Function: api_exec
Event-Calling-Line-Number: 1451
Event-Sequence: 2770
Job-UUID: 89227a8b-4e3c-4751-a424-332dfc979eed
Job-Command: originate
Job-Command-Arg: sofia/default/0706930821%20%26managed(MyApp)
Content-Length: 21

-ERR INVALID_PROFILE
*/
}
