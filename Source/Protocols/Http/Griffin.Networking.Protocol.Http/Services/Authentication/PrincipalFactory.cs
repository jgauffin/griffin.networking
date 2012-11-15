﻿using System.Security.Principal;
using Griffin.Networking.Http.Protocol;

namespace Griffin.Networking.Http.Services.Authentication
{
    /// <summary>
    /// Used to create <see cref="IPrincipal"/>
    /// </summary>
    public interface IPrincipalFactory
    {
        /// <summary>
        /// Create a new prinicpal
        /// </summary>
        /// <param name="context">Context used to identify the user.</param>
        /// <returns>Principal to use</returns>
        IPrincipal Create(PrincipalFactoryContext context);
    }

    public class PrincipalFactoryContext
    {
        /// <summary>
        /// Gets the user which was provided by the <see cref="IAuthenticateUserService"/>.
        /// </summary>
        public IAuthenticationUser User { get; set; }

        /// <summary>
        /// Gets the HTTP request.
        /// </summary>
        public IRequest Request { get; set; }
    }
}