using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Griffin.Networking.Http.Protocol;

namespace Griffin.Networking.Http.Services.Authentication
{
    class BasicAuthentication : IAuthenticator
    {
        private readonly IAuthenticateUserService _userService;
        private readonly string _realm;

        public BasicAuthentication(IAuthenticateUserService userService, string realm)
        {
            _userService = userService;
            _realm = realm;
        }

        /// <summary>
        /// Gets authenticator scheme
        /// </summary>
        /// <value></value>
        /// <example>
        /// digest
        /// </example>
        public string Scheme
        {
            get { return "basic"; }
        }

        /// <summary>
        /// Create a WWW-Authenticate header
        /// </summary>
        public void CreateChallenge(IRequest httpRequest, IResponse response)
        {
            response.AddHeader("WWW-Authenticate", "Basic realm=\"" + _realm + "\"");
        }

        /// <summary>
        /// Gets name of the authentication scheme
        /// </summary>
        /// <remarks>"BASIC", "DIGEST" etc.</remarks>
        public string AuthenticationScheme
        {
            get { return "basic"; }
        }

        /// <summary>
        /// Authenticate a request.
        /// </summary>
        /// <param name="request">Request being authenticated</param>
        /// <returns>Authenticated user if successful; otherwise null.</returns>
        public IAuthenticationUser Authenticate(IRequest request)
        {
            var authHeader = request.Headers["Authenticate"];
            if (authHeader == null)
                return null;

            /*
             * To receive authorization, the client sends the userid and password,
                separated by a single colon (":") character, within a base64 [7]
                encoded string in the credentials.*/
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Value));
            var pos = decoded.IndexOf(':');
            if (pos == -1)
                throw new BadRequestException("Invalid basic authentication header, failed to find colon. Got: " + authHeader.Value);

            string password = decoded.Substring(pos + 1, decoded.Length - pos - 1);
            string userName = decoded.Substring(0, pos);

            var user = _userService.Lookup(userName, request.Uri);
            if (user == null)
                return null;

            if (user.Password == null)
            {
                var ha1 = DigestAuthenticator.GetHa1(request.Uri.Host, userName, password);
                if (ha1 != user.HA1)
                    throw new HttpException(HttpStatusCode.Unauthorized, "Incorrect username or password");
            }
            else
            {
                if (password != user.Password)
                    throw new HttpException(HttpStatusCode.Unauthorized, "Incorrect username or password");
            }

            return user;
        }
    }
}
