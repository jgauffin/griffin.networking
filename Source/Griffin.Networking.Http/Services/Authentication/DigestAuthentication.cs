using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Griffin.Networking.Http.Implementation;
using Griffin.Networking.Http.Protocol;

namespace Griffin.Networking.Http.Services.Authentication
{
    public class DigestAuthentication : IAuthenticator
    {
        private static readonly Dictionary<string, DateTime> _nonces = new Dictionary<string, DateTime>();
        private static Timer _timer;

        /// <summary>
        /// Used by test classes to be able to use hardcoded values
        /// </summary>
        public static bool DisableNonceCheck;

        private readonly ILogger _logger = LogManager.GetLogger<DigestAuthentication>();
        private readonly IPrincipalFactory _principalFactory;
        private readonly IAuthenticateUserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DigestAuthentication"/> class.
        /// </summary>
        /// <param name="userService">Supplies users during authentication process.</param>
        public DigestAuthentication(IAuthenticateUserService userService, IPrincipalFactory principalFactory)
        {
            _userService = userService;
            _principalFactory = principalFactory;
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
            get { return "digest"; }
        }

        #region IAuthenticator Members

        /// <summary>
        /// Create a WWW-Authenticate header
        /// </summary>
        public void CreateChallenge(IRequest request, IResponse response)
        {
            var nonce = GetCurrentNonce();

            var challenge = new StringBuilder("Digest realm=\"");
            challenge.Append(request.Uri.Host);
            challenge.Append("\"");
            challenge.Append(", nonce=\"");
            challenge.Append(nonce);
            challenge.Append("\"");
            challenge.Append(", opaque=\"" + Guid.NewGuid().ToString().Replace("-", string.Empty) + "\"");
            challenge.Append(", stale=");

            /*if (options.Length > 0)
                challenge.Append((bool)options[0] ? "true" : "false");
            else*/
            challenge.Append("false");

            challenge.Append(", algorithm=MD5");
            challenge.Append(", qop=auth");


            response.AddHeader("WWW-Authenticate", challenge.ToString());
        }

        /// <summary>
        /// Gets name of the authentication scheme
        /// </summary>
        /// <remarks>"BASIC", "DIGEST" etc.</remarks>
        public string AuthenticationScheme
        {
            get { return "digest"; }
        }

        public void Authenticate(IRequest httpRequest)
        {
            var authHeader = httpRequest.Headers["Authenticate"];
            if (authHeader == null)
                return;


            lock (_nonces)
            {
                if (_timer == null)
                    _timer = new Timer(ManageNonces, null, 15000, 15000);
            }

            var parser = new NameValueParser();
            var parameters = new ParameterCollection();
            parser.Parse(authHeader.Value, parameters);
            if (!IsValidNonce(parameters["nonce"]) && !DisableNonceCheck)
                return;

            // request authentication information
            var username = parameters["username"];
            var user = _userService.Lookup(username, httpRequest.Uri);
            if (user == null)
                return;

            // Encode authentication info
            var HA1 = string.IsNullOrEmpty(user.HA1) ? GetHA1(httpRequest.Uri.Host, username, user.Password) : user.HA1;

            // encode challenge info
            var A2 = String.Format("{0}:{1}", httpRequest.Method, parameters["uri"]);
            var HA2 = GetMD5HashBinHex2(A2);
            var hashedDigest = Encrypt(HA1, HA2, parameters["qop"],
                                       parameters["nonce"], parameters["nc"], parameters["cnonce"]);

            //validate
            if (parameters["response"] == hashedDigest)
            {
                var principal =
                    _principalFactory.Create(new PrincipalFactoryContext {Request = httpRequest, User = user});
                Thread.CurrentPrincipal = principal;
            }
            else
                throw new HttpException(HttpStatusCode.Unauthorized, "Failed to authenticate");
        }

        #endregion

        /// <summary>
        /// Encrypts parameters into a Digest string
        /// </summary>
        /// <param name="realm">Realm that the user want to log into.</param>
        /// <param name="userName">User logging in</param>
        /// <param name="password">Users password.</param>
        /// <param name="method">HTTP method.</param>
        /// <param name="uri">Uri/domain that generated the login prompt.</param>
        /// <param name="qop">Quality of Protection.</param>
        /// <param name="nonce">"Number used ONCE"</param>
        /// <param name="nc">Hexadecimal request counter.</param>
        /// <param name="cnonce">"Client Number used ONCE"</param>
        /// <returns>Digest encrypted string</returns>
        public static string Encrypt(string realm, string userName, string password, string method, string uri,
                                     string qop, string nonce, string nc, string cnonce)
        {
            var HA1 = GetHA1(realm, userName, password);
            var A2 = String.Format("{0}:{1}", method, uri);
            var HA2 = GetMD5HashBinHex2(A2);

            string unhashedDigest;
            if (qop != null)
            {
                unhashedDigest = String.Format("{0}:{1}:{2}:{3}:{4}:{5}",
                                               HA1,
                                               nonce,
                                               nc,
                                               cnonce,
                                               qop,
                                               HA2);
            }
            else
            {
                unhashedDigest = String.Format("{0}:{1}:{2}",
                                               HA1,
                                               nonce,
                                               HA2);
            }

            return GetMD5HashBinHex2(unhashedDigest);
        }

        public static string GetHA1(string realm, string userName, string password)
        {
            return GetMD5HashBinHex2(String.Format("{0}:{1}:{2}", userName, realm, password));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ha1">Md5 hex encoded "userName:realm:password", without the quotes.</param>
        /// <param name="ha2">Md5 hex encoded "method:uri", without the quotes</param>
        /// <param name="qop">Quality of Protection</param>
        /// <param name="nonce">"Number used ONCE"</param>
        /// <param name="nc">Hexadecimal request counter.</param>
        /// <param name="cnonce">Client number used once</param>
        /// <returns></returns>
        protected virtual string Encrypt(string ha1, string ha2, string qop, string nonce, string nc, string cnonce)
        {
            string unhashedDigest;
            if (qop != null)
            {
                unhashedDigest = String.Format("{0}:{1}:{2}:{3}:{4}:{5}",
                                               ha1,
                                               nonce,
                                               nc,
                                               cnonce,
                                               qop,
                                               ha2);
            }
            else
            {
                unhashedDigest = String.Format("{0}:{1}:{2}",
                                               ha1,
                                               nonce,
                                               ha2);
            }

            return GetMD5HashBinHex2(unhashedDigest);
        }

        private void ManageNonces(object state)
        {
            try
            {
                lock (_nonces)
                {
                    foreach (var pair in _nonces)
                    {
                        if (pair.Value >= DateTime.Now)
                            continue;

                        _nonces.Remove(pair.Key);
                        return;
                    }
                }
            }
            catch (Exception err)
            {
                _logger.Error("Failed to manage nonces.", err);
            }
        }

        /// <summary>
        /// Gets the current nonce.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetCurrentNonce()
        {
            var nonce = Guid.NewGuid().ToString().Replace("-", string.Empty);
            lock (_nonces)
                _nonces.Add(nonce, DateTime.Now.AddSeconds(30));

            return nonce;
        }

        /// <summary>
        /// Gets the Md5 hash bin hex2.
        /// </summary>
        /// <param name="toBeHashed">To be hashed.</param>
        /// <returns></returns>
        public static string GetMD5HashBinHex2(string toBeHashed)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            var result = md5.ComputeHash(Encoding.Default.GetBytes(toBeHashed));

            var sb = new StringBuilder();
            foreach (var b in result)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        /// <summary>
        /// determines if the nonce is valid or has expired.
        /// </summary>
        /// <param name="nonce">nonce value (check wikipedia for info)</param>
        /// <returns><c>true</c> if the nonce has not expired.</returns>
        protected virtual bool IsValidNonce(string nonce)
        {
            lock (_nonces)
            {
                if (_nonces.ContainsKey(nonce))
                {
                    if (_nonces[nonce] < DateTime.Now)
                    {
                        _nonces.Remove(nonce);
                        return false;
                    }

                    return true;
                }
            }

            return false;
        }
    }
}