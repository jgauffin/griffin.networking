using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Autofac;
using Griffin.Networking.Http.Handlers;
using Griffin.Networking.Http.Implementation;
using Griffin.Networking.Http.Messages;
using Griffin.Networking.Http.Pipeline.Handlers;
using Griffin.Networking.Http.Services.Authentication;
using Griffin.Networking.Http.Services.BodyDecoders;
using Griffin.Networking.Http.Services.Errors;
using Griffin.Networking.Logging;
using Griffin.Networking.Messaging;
using Griffin.Networking.Pipelines;
using Griffin.Networking.Servers;

namespace Griffin.Networking.Http.DemoServer
{
    public class Program
    {
        
        

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            LogManager.Assign(new SimpleLogManager<ConsoleLogger>());



            var server = new MessagingServer(new MyClientFactory(), new MessagingServerConfiguration(new HttpMessageFactory()));
            server.Start(new IPEndPoint(IPAddress.Loopback, 8888));
            Console.Read();

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

            factory.AddUpstreamHandler(() => new HeaderDecoder());
            factory.AddUpstreamHandler(new HttpErrorHandler(new SimpleErrorFormatter()));
            factory.AddUpstreamHandler(authHandler);
            factory.AddUpstreamHandler(() => new BodyDecoder(new CompositeBodyDecoder(), 65535, 6000000));
            //factory.AddUpstreamHandler(() => new FileHandler());
            factory.AddUpstreamHandler(() => new MessageHandler());
            //factory.AddUpstreamHandler(new PipelineFailureHandler());

            HttpListener listener = new HttpListener(10);
            listener.Start(new IPEndPoint(IPAddress.Any, 8080));

            Console.ReadLine();
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {

        }
    }
}
