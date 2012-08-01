using System;
using Griffin.Networking.Protocol.FreeSwitch.Events;

namespace Griffin.Networking.Protocol.FreeSwitch
{
    /// <summary>
    /// Used by <see cref="FreeSwitchClient.EventReceived"/> when an event has been received.
    /// </summary>
    public class EventRecievedEventArgs : EventArgs
    {
        private readonly EventBase _receivedEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventRecievedEventArgs"/> class.
        /// </summary>
        /// <param name="receivedEvent">The received event.</param>
        public EventRecievedEventArgs(EventBase receivedEvent)
        {
            _receivedEvent = receivedEvent;
        }

        /// <summary>
        /// Gets the received event.
        /// </summary>
        public EventBase ReceivedEvent
        {
            get { return _receivedEvent; }
        }
    }
}