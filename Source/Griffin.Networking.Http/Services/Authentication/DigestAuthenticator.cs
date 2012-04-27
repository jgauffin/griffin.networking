using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Griffin.Networking.Http.Implementation;
using Griffin.Networking.Http.Protocol;
using Griffin.Networking.Logging;

namespace Griffin.Networking.Http.Services.Authentication
{
    /// <summary>
    /// Implements Digest authentication as described in wikipedia.
    /// </summary>
    public class DigestAuthenticator : IAuthenticator
    {
        private static readonly Dictionary<string, DateTime> Nonces = new Dictionary<string, DateTime>();
        private static Timer _timer;

        /// <summary>
        /// Used by test classes to be able to use hardcoded values
        /// </summary>
        public static bool DisableNonceCheck = true;

        private readonly ILogger _logger = LogManager.GetLogger<DigestAuthenticator>();
        private readonly IRealmRepository _realmRepository;
        private readonly IAuthenticateUserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DigestAuthenticator"/> class.
        /// </summary>
        /// <param name="realmRepository">Used to lookup the realm for a HTTP request</param>
        /// <param name="userService">Supplies users during authentication process.</param>
        public DigestAuthenticator(IRealmRepository realmRepository, IAuthenticateUserService userService)
        {
            _realmRepository = realmRepository;
            _userService = userService;
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
            challenge.Append(_realmRepository.GetRealm(request));
            challenge.Append("\"");
            challenge.Append(", nonce=\"");
            challenge.Append(nonce);
            challenge.Append("\"");
            challenge.Append(", opaque=\"" + Guid.NewGuid().ToString().Replace("-", string.Empty) + "\"");

            /* Disable the stale mechanism
             * We should really generate the responses directly in these classes.
            challenge.Append(", stale=");
            challenge.Append((bool)options[0] ? "true" : "false");
            challenge.Append("false");
             * */

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

        public IAuthenticationUser Authenticate(IRequest request)
        {
            var authHeader = request.Headers["Authorization"];
            if (authHeader == null)
                return null;


            if (_timer == null)
            {
                lock (Nonces)
                {
                    if (_timer == null)
                        _timer = new Timer(ManageNonces, null, 15000, 15000);
                }
            }

            var parser = new NameValueParser();
            var parameters = new ParameterCollection();
            parser.Parse(authHeader.Value.Remove(0, AuthenticationScheme.Length + 1), parameters);
            if (!IsValidNonce(parameters["nonce"]) && !DisableNonceCheck)
                throw new HttpException(HttpStatusCode.Unauthorized, "Invalid nonce.");

            // request authentication information
            var username = parameters["username"];
            var user = _userService.Lookup(username, request.Uri);
            if (user == null)
                return null;

            // Encode authentication info
            var ha1 = string.IsNullOrEmpty(user.HA1) ? GetHa1(_realmRepository.GetRealm(request), username, user.Password) : user.HA1;

            // encode challenge info
            var a2 = String.Format("{0}:{1}", request.Method, request.Uri.AbsolutePath);
            var ha2 = GetMd5HashBinHex(a2);
            var hashedDigest = Encrypt(ha1, ha2, parameters["qop"],
                                       parameters["nonce"], parameters["nc"], parameters["cnonce"]);

            //validate
            if (parameters["response"] == hashedDigest)
            {
                return user;
            }

            return null;
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
            var ha1 = GetHa1(realm, userName, password);
            var a2 = String.Format("{0}:{1}", method, uri);
            var ha2 = GetMd5HashBinHex(a2);

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

            return GetMd5HashBinHex(unhashedDigest);
        }

        public static string GetHa1(string realm, string userName, string password)
        {
            return GetMd5HashBinHex(String.Format("{0}:{1}:{2}", userName, realm, password));
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

            return GetMd5HashBinHex(unhashedDigest);
        }

        private void ManageNonces(object state)
        {
            try
            {
                lock (Nonces)
                {
                    foreach (var pair in Nonces)
                    {
                        if (pair.Value >= DateTime.Now)
                            continue;

                        Nonces.Remove(pair.Key);
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
            lock (Nonces)
                Nonces.Add(nonce, DateTime.Now.AddSeconds(30));

            return nonce;
        }

        /// <summary>
        /// Gets the Md5 hash bin hex2.
        /// </summary>
        /// <param name="toBeHashed">To be hashed.</param>
        /// <returns></returns>
        public static string GetMd5HashBinHex(string toBeHashed)
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
            lock (Nonces)
            {
                if (Nonces.ContainsKey(nonce))
                {
                    if (Nonces[nonce] < DateTime.Now)
                    {
                        Nonces.Remove(nonce);
                        return false;
                    }

                    return true;
                }
            }

            return false;
        }
    }
}