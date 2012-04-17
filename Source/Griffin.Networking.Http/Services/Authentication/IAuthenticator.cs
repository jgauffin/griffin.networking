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

        /// <summary>
        /// Authenticate a request.
        /// </summary>
        /// <param name="request">Request being authenticated</param>
        /// <returns>Authenticated user if successful; otherwise null.</returns>
        IAuthenticationUser Authenticate(IRequest request);
    }
}
