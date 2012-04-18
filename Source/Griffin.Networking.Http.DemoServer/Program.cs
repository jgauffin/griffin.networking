using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Autofac;
using Griffin.Networking.Http.Handlers;
using Griffin.Networking.Http.Implementation;
using Griffin.Networking.Http.Services.BodyDecoders;
using Griffin.Networking.Pipelines;

namespace Griffin.Networking.Http.DemoServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            //LogManager.Assign(new ConsoleLogManager());
            /*
            var cb = new ContainerBuilder();
            cb.RegisterType<HttpParser>().AsImplementedInterfaces().SingleInstance();
            cb.RegisterType<ResponseEncoder>().AsSelf().SingleInstance();
            cb.RegisterType<HeaderDecoder>().AsSelf().SingleInstance();
            cb.RegisterType<MessageHandler>().AsSelf().SingleInstance();
            cb.RegisterType<FileHandler>().AsSelf().SingleInstance();
            var serviceLocator = new AutofacServiceLocator(cb.Build());
            */
            var factory = new DelegatePipelineFactory();
            factory.AddDownstreamHandler(() => new ResponseEncoder());
            //factory.AddDownstreamHandler(new BufferTracer());
            //factory.AddUpstreamHandler(new BufferTracer());
            factory.AddUpstreamHandler(() => new HeaderDecoder(new HttpParser()));
            factory.AddUpstreamHandler(() => new BodyDecoder(new CompositeBodyDecoder(), 65535, 6000000));
            //factory.AddUpstreamHandler(() => new FileHandler());
            factory.AddUpstreamHandler(() => new MessageHandler());

            HttpListener listener = new HttpListener(factory);
            listener.Start(new IPEndPoint(IPAddress.Any, 8080));

            Console.ReadLine();
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            
        }
    }
}
