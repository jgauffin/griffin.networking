using System;

namespace Griffin.Networking.Protocol.FreeSwitch.Events
{
    /// <summary>
    /// Generate event classes
    /// </summary>
    public interface IEventFactory
    {
        /// <summary>
        /// Register a class for a FreeSwitch Event
        /// </summary>
        /// <param name="eventName">Name as defined by FreeSWITCH</param>
        /// <param name="eventType">Class type. Must have a parameterless constructor.</param>
        void Register(string eventName, Type eventType);

        /// <summary>
        /// Create a new event object
        /// </summary>
        /// <param name="eventName">FreeSWITCH event name</param>
        /// <returns>Event if name was mapped; otherwise null.</returns>
        EventBase Create(string eventName);

        /// <summary>
        /// Create a new event object (custom event)
        /// </summary>
        /// <param name="eventName">FreeSWITCH event name</param>
        /// <returns>Event if name was mapped; otherwise null.</returns>
        EventBase CreateCustom(string eventName);
    }
}