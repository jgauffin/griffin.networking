using Griffin.Networking.Protocol.FreeSwitch.Events;

namespace Griffin.Networking.Protocol.FreeSwitch.Net.Messages
{
    /// <summary>
    /// An event have been recieved from FreeSWITCH.
    /// </summary>
    public class EventRecieved : IPipelineMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventRecieved"/> class.
        /// </summary>
        /// <param name="fsEvent">The fs event.</param>
        public EventRecieved(EventBase fsEvent)
        {
            FreeSwitchEvent = fsEvent;
        }

        /// <summary>
        /// Gets event recieved from freeswitch
        /// </summary>
        public EventBase FreeSwitchEvent { get; private set; }
    }
}