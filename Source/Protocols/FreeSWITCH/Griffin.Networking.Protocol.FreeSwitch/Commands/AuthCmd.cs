using System.Security;

namespace Griffin.Networking.Protocol.FreeSwitch.Commands
{
    /// <summary>
    /// authentication command
    /// </summary>
    public class AuthCmd : ICommand
    {
        private readonly SecureString _password;

        public AuthCmd(SecureString password)
        {
            _password = password;
        }

        #region ICommand Members

        public string ToFreeSwitchString()
        {
            return string.Format("auth {0}", _password.ToClearText());
        }

        #endregion
    }
}