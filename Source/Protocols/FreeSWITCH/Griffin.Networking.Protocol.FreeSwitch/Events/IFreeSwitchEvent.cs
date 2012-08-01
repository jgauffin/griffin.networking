using System;

namespace Griffin.Networking.Protocol.FreeSwitch.Events
{
    /// <summary>
    /// Event recieved from FreeSWITCH
    /// </summary>
    public interface IFreeSwitchEvent
    {
        /// <summary>
        /// Gets FreeSwitch server that the event is for.
        /// </summary>
        UniqueId CoreId { get; set; }

        /// <summary>
        /// Gets local date when the event was generated
        /// </summary>
        DateTime LocalDate { get; set; }

        /// <summary>
        /// Gets UTC date when the event was generated.
        /// </summary>
        DateTime UtcDate { get; set; }
    }
}