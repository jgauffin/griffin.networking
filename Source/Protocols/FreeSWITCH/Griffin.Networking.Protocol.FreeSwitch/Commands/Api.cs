using System.Collections.Generic;

namespace Griffin.Networking.Protocol.FreeSwitch.Commands
{
    public class Api : ICommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Api"/> class.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="arguments">Command arguments.</param>
        public Api(string commandName, params string[] arguments)
        {
            CommandName = commandName;
            Arguments = arguments;
        }

        public virtual string CommandName { get; private set; }
        public virtual IEnumerable<string> Arguments { get; private set; }

        #region ICommand Members

        /// <summary>
        /// Convert command to a string that can be sent to FreeSWITCH
        /// </summary>
        /// <returns>FreeSWITCH command</returns>
        public virtual string ToFreeSwitchString()
        {
            return string.Format("api {0}", BuildApiString());
        }

        #endregion

        protected virtual string BuildApiString()
        {
            return string.Format("{0} {1}", CommandName, string.Join(" ", Arguments));
        }
    }
}