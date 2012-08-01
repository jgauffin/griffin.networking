using System.Linq;
using Griffin.Networking.Protocol.FreeSwitch.Events;

namespace Griffin.Networking.Protocol.FreeSwitch.Commands
{
    public class SendEventCmd : ICommand
    {
        private readonly EventBase _event;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evt">Note that all events have NOT being developed to be sent, only to be received.</param>
        public SendEventCmd(EventBase evt)
        {
            _event = evt;
        }

        #region ICommand Members

        /// <summary>
        /// Convert command to a string that can be sent to FreeSWITCH
        /// </summary>
        /// <returns>FreeSWITCH command</returns>
        public string ToFreeSwitchString()
        {
            var names = _event.GetType().GetEventNames();
            return string.Format("sendevent {0}", names.First());
        }

        #endregion
    }
}