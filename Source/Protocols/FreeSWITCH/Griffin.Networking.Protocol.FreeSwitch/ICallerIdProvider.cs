using Griffin.Networking.Protocol.FreeSwitch.Commands;

namespace Griffin.Networking.Protocol.FreeSwitch
{
    /// <summary>
    /// Allows one to set caller id (context depends on the type of command)
    /// </summary>
    /// <remarks>Used by for instance <see cref="Originate"/> command to set outbound caller id.</remarks>
    public interface ICallerIdProvider
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        string CallerIdName { get; set; }

        /// <summary>
        /// Gets or sets the number, must be formatted per usage area (for instance per the trunk rules for outbound external calls)
        /// </summary>
        string CallerIdNumber { get; set; }
    }
}