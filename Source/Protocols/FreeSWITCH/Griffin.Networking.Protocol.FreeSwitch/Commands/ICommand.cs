namespace Griffin.Networking.Protocol.FreeSwitch.Commands
{
    /// <summary>
    /// A freeswitch command
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Convert command to a string that can be sent to FreeSWITCH
        /// </summary>
        /// <returns>FreeSWITCH command</returns>
        string ToFreeSwitchString();
    }
}