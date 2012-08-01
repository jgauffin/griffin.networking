namespace Griffin.Networking.Protocol.FreeSwitch.Commands
{
    public class ParkCmd : ICommand
    {
        private readonly UniqueId _uuid;

        public ParkCmd(UniqueId id)
        {
            _uuid = id;
        }

        #region ICommand Members

        /// <summary>
        /// Convert command to a string that can be sent to FreeSWITCH
        /// </summary>
        /// <returns>FreeSWITCH command</returns>
        public string ToFreeSwitchString()
        {
            return string.Format("uuid_park {0}", _uuid);
        }

        #endregion
    }
}