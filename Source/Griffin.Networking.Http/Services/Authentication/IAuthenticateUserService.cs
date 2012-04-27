using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace Griffin.Networking.Http.Services.Authentication
{
    /// <summary>
    /// Provider returning user to be authenticated.
    /// </summary>
    public interface IAuthenticateUserService
    {
        /// <summary>
        /// Lookups the specified user
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <param name="host">Typically web server domain name.</param>
        /// <returns>User if found; otherwise <c>null</c>.</returns>
        /// <remarks>
        /// User name can basically be anything. For instance name entered by user when using
        /// basic or digest authentication, or SID when using Windows authentication.
        /// </remarks>
        IAuthenticationUser Lookup(string userName, Uri host);
        /*
        /// <summary>
        /// Gets the principal to use.
        /// </summary>
        /// <param name="user">Successfully authenticated user.</param>
        /// <returns></returns>
        /// <remarks>
        /// Invoked when a user have successfully been authenticated.
        /// </remarks>
        /// <seealso cref="GenericPrincipal"/>
        /// <seealso cref="WindowsPrincipal"/>
        IPrincipal GetPrincipal(IAuthenticationUser user);
         */
    }
}
