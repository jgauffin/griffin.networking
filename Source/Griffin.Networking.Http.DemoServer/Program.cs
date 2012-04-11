using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Autofac;
using Griffin.Networking.Http.Implementation;
using Griffin.Networking.Pipelines;

namespace Griffin.Networking.Http.DemoServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var cb = new ContainerBuilder();
            cb.RegisterType<HttpParser>().AsImplementedInterfaces().InstancePerLifetimeScope();
            cb.RegisterType<Encoder>().AsSelf().InstancePerLifetimeScope();
            cb.RegisterType<Decoder>().AsSelf().InstancePerLifetimeScope();
            cb.RegisterType<MessageHandler>().AsSelf().InstancePerLifetimeScope();
            var serviceLocator = new AutofacServiceLocator(cb.Build());

            var factory = new ServiceLocatorPipelineFactory(serviceLocator);
            factory.AddDownstreamHandler<Encoder>();
            factory.AddUpstreamHandler<Decoder>();
            factory.AddUpstreamHandler<MessageHandler>();

            HttpListener listener = new HttpListener(factory);
            listener.Start(new IPEndPoint(IPAddress.Any, 8080));

            Console.ReadLine();
        }
    }
}
