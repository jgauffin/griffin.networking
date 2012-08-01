using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Griffin.Networking.Protocol.FreeSwitch.Events
{
    /// <summary>
    /// Used to load and create events
    /// </summary>
    public class EventFactory : IEventFactory
    {
        private readonly Dictionary<string, Type> _eventTypes =
            new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        #region IEventFactory Members

        /// <summary>
        /// Register a class for a FreeSwitch Event
        /// </summary>
        /// <param name="eventName">Name as defined by FreeSWITCH</param>
        /// <param name="eventType">Class type. Must have a parameterless constructor.</param>
        public void Register(string eventName, Type eventType)
        {
            if (!typeof (EventBase).IsAssignableFrom(eventType))
                throw new ArgumentException(eventType.FullName + " does not subclass Event.");

            _eventTypes.Add(eventName, eventType);
        }

        /// <summary>
        /// Create a new event object
        /// </summary>
        /// <param name="eventName">FreeSWITCH event name</param>
        /// <returns>
        /// Event if name was mapped; otherwise null.
        /// </returns>
        public EventBase Create(string eventName)
        {
            Type type;
            if (!_eventTypes.TryGetValue(eventName, out type))
                return null;

            return (EventBase) Activator.CreateInstance(type);
        }

        /// <summary>
        /// Create a new event object
        /// </summary>
        /// <param name="eventName">FreeSWITCH event name</param>
        /// <returns>
        /// Event if name was mapped; otherwise null.
        /// </returns>
        public EventBase CreateCustom(string eventName)
        {
            Type type;
            if (!_eventTypes.TryGetValue("custom::" + eventName, out type))
                return null;

            return (EventBase)Activator.CreateInstance(type);
        }

        #endregion

        /// <summary>
        /// Map all event classes in this assembly
        /// </summary>
        public void MapDefault()
        {
            var eventType = typeof (EventBase);
            foreach (
                var type in
                    Assembly.GetExecutingAssembly().GetTypes().Where(t => eventType.IsAssignableFrom(t) && !t.IsAbstract)
                )
            {
                var attributes = type.GetCustomAttributes(typeof (EventNameAttribute), false).Cast<EventNameAttribute>();
                foreach (var attribute in attributes)
                {
                    if (attribute.IsCustom)
                        _eventTypes.Add("custom::" + attribute.Name, type);
                    else
                        _eventTypes.Add(attribute.Name, type);
                }
            }
        }
    }
}