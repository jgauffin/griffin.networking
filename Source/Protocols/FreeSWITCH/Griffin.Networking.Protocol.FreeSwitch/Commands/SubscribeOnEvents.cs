using Griffin.Networking.Protocol.FreeSwitch.Net;

namespace Griffin.Networking.Protocol.FreeSwitch.Commands
{
    /// <summary>
    /// Used to subscribe on events
    /// </summary>
    public class SubscribeOnEvents : ICommand
    {
        private readonly EventSubscriptionType _eventSubscriptionType;
        private readonly FreeSwitchEventCollection _events;

        public SubscribeOnEvents(EventSubscriptionType eventSubscriptionType, FreeSwitchEventCollection events)
        {
            _eventSubscriptionType = eventSubscriptionType;
            _events = events;
        }

        #region ICommand Members

        /// <summary>
        /// Convert command to a string that can be sent to FreeSWITCH
        /// </summary>
        /// <returns>FreeSWITCH command</returns>
        public string ToFreeSwitchString()
        {
            var events = string.Empty;
            for (var i = 0; i < _events.Count; ++i)
                events += _events[i].ToString().CamelCaseToUpperCase() + " ";

            return "event " + _eventSubscriptionType.ToString().ToLower() + " " + events;
        }

        #endregion
    }
}