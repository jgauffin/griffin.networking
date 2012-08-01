using System;

namespace Griffin.Networking.Protocol.FreeSwitch.Events
{
    /// <summary>
    /// Used to map .NET classes to FreeSwitch events.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class EventNameAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventNameAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public EventNameAttribute(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            Name = name;
        }

        /// <summary>
        /// Gets if this is a FreeSWITCH custom event.
        /// </summary>
        public bool IsCustom { get; set; }

        /// <summary>
        /// Gets name of event
        /// </summary>
        public string Name { get; private set; }
    }
}