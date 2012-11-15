using System;
using System.Security.Principal;
using Griffin.Networking.Http.Services.Authentication;

namespace Griffin.Networking.Http.DemoServer.ThroughPipeline
{
    public class DummyAuthenticatorService : IAuthenticateUserService, IPrincipalFactory
    {
        #region IAuthenticateUserService Members

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
        public IAuthenticationUser Lookup(string userName, Uri host)
        {
            return new SimpleUser
                {
                    HA1 = null,
                    Password = "Svenne",
                    Username = "Jonas"
                };
        }

        #endregion

        #region IPrincipalFactory Members

        /// <summary>
        /// Create a new prinicpal
        /// </summary>
        /// <param name="context">Context used to identify the user.</param>
        /// <returns>
        /// Principal to use
        /// </returns>
        public IPrincipal Create(PrincipalFactoryContext context)
        {
            return new GenericPrincipal(new GenericIdentity(context.User.Username), new string[0]);
        }

        #endregion

        #region Nested type: SimpleUser

        private class SimpleUser : IAuthenticationUser
        {
            #region IAuthenticationUser Members

            /// <summary>
            /// Gets or sets user name used during authentication.
            /// </summary>
            public string Username { get; set; }

            /// <summary>
            /// Gets or sets unencrypted password.
            /// </summary>
            /// <remarks>
            /// Password as clear text. You could use <see cref="HA1"/> instead if your passwords
            /// are encrypted in the database.
            /// </remarks>
            public string Password { get; set; }

            /// <summary>
            /// Gets or sets HA1 hash.
            /// </summary>
            /// <remarks>
            /// <para>
            /// Digest authentication requires clear text passwords to work. If you
            /// do not have that, you can store a HA1 hash in your database (which is part of
            /// the Digest authentication process).
            /// </para>
            /// <para>
            /// A HA1 hash is simply a Md5 encoded string: "UserName:Realm:Password". The quotes should
            /// not be included. Realm is the currently requested Host (as in <c>Request.Headers["host"]</c>).
            /// </para>
            /// <para>
            /// Leave the string as <c>null</c> if you are not using HA1 hashes.
            /// </para>
            /// </remarks>
            public string HA1 { get; set; }

            #endregion
        }

        #endregion
    }
}