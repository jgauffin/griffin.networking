using Griffin.Networking.Http.Protocol;

namespace Griffin.Networking.Http.Services.Authentication
{
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