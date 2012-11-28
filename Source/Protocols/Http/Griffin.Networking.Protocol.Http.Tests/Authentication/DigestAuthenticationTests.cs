using System;
using Griffin.Networking.Protocol.Http.Protocol;
using Griffin.Networking.Protocol.Http.Services.Authentication;
using NSubstitute;
using Xunit;

namespace Griffin.Networking.Protocol.Http.Tests.Authentication
{
    public class DigestAuthenticatorTests
    {
        /// <summary>
        /// Values are taken from the Wikipedia Article
        /// </summary>
        [Fact]
        public void Test()
        {
            var uri = new Uri("http://testrealm@host.com/dir/index.html");
            var headerValue =
                @"Digest username=""Mufasa"", realm=""testrealm@host.com"", nonce=""dcd98b7102dd2f0e8b11d0f600bfb0c093"", uri=""/dir/index.html"", qop=auth, nc=00000001, cnonce=""0a4f113b"", response=""6629fae49393a05397450978507c4ef1"", opaque=""5ccc069c403ebaf9f0171e9517f40e41";
            var mock = Substitute.For<IAccountStorage>();
            mock.Lookup("Mufasa", uri).Returns(new AuthenticationUserStub
                {Username = "Mufasa", Password = "Circle Of Life"});
            var realmRepos = Substitute.For<IRealmRepository>();
            realmRepos.GetRealm(Arg.Any<IRequest>()).Returns("testrealm@host.com");
            var auth = new DigestAuthenticator(realmRepos, mock);
            var request = Substitute.For<IRequest>();
            request.Headers["Authorization"].Returns(new HeaderItemStub {Name = "Authorization", Value = headerValue});
            request.Uri.Returns(uri);
            request.Method.Returns("GET");

            var user = auth.Authenticate(request);

            Assert.NotNull(user);
        }
    }

    public class HeaderItemStub : IHeaderItem
    {
        #region IHeaderItem Members

        /// <summary>
        /// Gets header name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets value as it would be sent back to client.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Does a case insensitive compare with the specified value
        /// </summary>
        /// <param name="value">Value to compare our value with</param>
        /// <returns>true if equal; otherwase false;</returns>
        public bool Is(string value)
        {
            return true;
        }

        /// <summary>
        /// Checks if the header has the specified parameter
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <returns>true if equal; otherwase false;</returns>
        public bool HasParameter(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get a parameter from the header
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetParameter(string name)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class AuthenticationUserStub : IAuthenticationUser
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
        /// Password as clear text. You could use <see cref="IAuthenticationUser.HA1"/> instead if your passwords
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
}