using Griffin.Networking.Http.Protocol;

namespace Griffin.Networking.Http.Services.Authentication
{
    /// <summary>
    /// Used to authenticate the user.
    /// </summary>
    public interface IAuthenticator
    {
        /// <summary>
        /// Gets name of the authentication scheme
        /// </summary>
        /// <remarks>"BASIC", "DIGEST" etc.</remarks>
        string AuthenticationScheme { get; }

        /// <summary>
        /// Create a WWW-Authorize header
        /// </summary>
        void CreateChallenge(IRequest httpRequest, IResponse response);

        /// <summary>
        /// Authorize a request.
        /// </summary>
        /// <param name="request">Request being authenticated</param>
        /// <returns>Authenticated user if successful; otherwise null.</returns>
        /// <exception cref="HttpException">403 Forbidden if the nonce is incorrect.</exception>
        IAuthenticationUser Authenticate(IRequest request);
    }
}