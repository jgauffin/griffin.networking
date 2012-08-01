using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using Autofac;
using Griffin.Networking.Http.Handlers;
using Griffin.Networking.Http.Implementation;
using Griffin.Networking.Http.Messages;
using Griffin.Networking.Http.Protocol;
using Griffin.Networking.Http.Services.Authentication;
using Griffin.Networking.Http.Services.BodyDecoders;
using Griffin.Networking.Http.Services.Errors;
using Griffin.Networking.Logging;
using Griffin.Networking.Pipelines;

namespace Griffin.Networking.Http.DemoServer
{
    public class Program
    {
        public class DummyAuthenticatorService : IAuthenticateUserService, IPrincipalFactory
        {
            class SimpleUser : IAuthenticationUser
            {
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
            }
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
        }

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            LogManager.Assign(new SimpleLogManager<ConsoleLogger>());
            /*
            var cb = new ContainerBuilder();
            cb.RegisterType<HttpParser>().AsImplementedInterfaces().SingleInstance();
            cb.RegisterType<ResponseEncoder>().AsSelf().SingleInstance();
            cb.RegisterType<HeaderDecoder>().AsSelf().SingleInstance();
            cb.RegisterType<MessageHandler>().AsSelf().SingleInstance();
            cb.RegisterType<FileHandler>().AsSelf().SingleInstance();
            var serviceLocator = new AutofacServiceLocator(cb.Build());
            */

            var authService = new DummyAuthenticatorService();
            var authHandler = new AuthenticationHandler(new DigestAuthenticator(new SingleRealmRepository("DragonsDen@wedone.it"), authService), authService);

            var factory = new DelegatePipelineFactory();
            factory.AddDownstreamHandler(authHandler);
            factory.AddDownstreamHandler(() => new ResponseEncoder());

            factory.AddUpstreamHandler(() => new HeaderDecoder(new HttpParser()));
            factory.AddUpstreamHandler(new HttpErrorHandler(new SimpleErrorFormatter()));
            factory.AddUpstreamHandler(authHandler);
            factory.AddUpstreamHandler(() => new BodyDecoder(new CompositeBodyDecoder(), 65535, 6000000));
            //factory.AddUpstreamHandler(() => new FileHandler());
            factory.AddUpstreamHandler(() => new MessageHandler());
            //factory.AddUpstreamHandler(new PipelineFailureHandler());

            HttpListener listener = new HttpListener(factory);
            listener.Start(new IPEndPoint(IPAddress.Any, 8080));

            Console.ReadLine();
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {

        }
    }
}
