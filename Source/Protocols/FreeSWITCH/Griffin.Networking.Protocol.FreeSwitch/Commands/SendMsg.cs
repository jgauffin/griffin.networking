using System;

namespace Griffin.Networking.Protocol.FreeSwitch.Commands
{
    public abstract class SendMsg : ICommand
    {
        private readonly string _callCommand = "execute";
        private readonly UniqueId _id;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendMsg"/> class.
        /// </summary>
        /// <param name="id">Channel UUID.</param>
        protected SendMsg(UniqueId id)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            _id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SendMsg"/> class.
        /// </summary>
        /// <param name="id">Channel UUID.</param>
        /// <param name="callCommand">The call command.</param>
        protected SendMsg(UniqueId id, string callCommand)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            if (string.IsNullOrEmpty(callCommand))
                throw new ArgumentNullException("callCommand");

            _callCommand = callCommand;
            _id = id;
        }

        public virtual string Command { get; private set; }
        public virtual string[] Arguments { get; private set; }

        #region ICommand Members

        public virtual string ToFreeSwitchString()
        {
            return
                string.Format("SendMsg {0}\ncall-command: {1}\nexecute-app-name: {2}\nexecute-app-arg: {3}\n", _id,
                              _callCommand, Command, Arguments);
        }

        #endregion
    }
}