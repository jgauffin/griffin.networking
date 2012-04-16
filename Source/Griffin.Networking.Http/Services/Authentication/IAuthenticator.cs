using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Networking.Http.Protocol;

namespace Griffin.Networking.Http.Services.Authentication
{
    /// <summary>
    /// Used to authenticate the user.
    /// </summary>
    public interface IAuthenticator
    {
        /// <summary>
        /// Create a WWW-Authenticate header
        /// </summary>
        void CreateChallenge(IRequest httpRequest, IResponse response);

        /// <summary>
        /// Gets name of the authentication scheme
        /// </summary>
        /// <remarks>"BASIC", "DIGEST" etc.</remarks>
        string AuthenticationScheme { get; }

        void Authenticate(IRequest httpRequest);
    }
}
