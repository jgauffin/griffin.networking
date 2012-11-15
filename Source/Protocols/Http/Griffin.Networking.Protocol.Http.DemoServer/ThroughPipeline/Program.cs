using System.Net;
using Griffin.Networking.Http.Handlers;
using Griffin.Networking.Http.Pipeline.Handlers;
using Griffin.Networking.Http.Services.Authentication;
using Griffin.Networking.Http.Services.BodyDecoders;
using Griffin.Networking.Http.Services.Errors;
using Griffin.Networking.Pipelines;
using Griffin.Networking.Servers;

namespace Griffin.Networking.Http.DemoServer.ThroughPipeline
{
    internal class Program
    {
        public static void RunDemo()
        {
            var authService = new DummyAuthenticatorService();
            var authHandler =
                new AuthenticationHandler(
                    new DigestAuthenticator(new SingleRealmRepository("DragonsDen@wedone.it"), authService), authService);

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

            var server = new PipelineServer(factory, new ServerConfiguration());
            server.Start(new IPEndPoint(IPAddress.Any, 8888));
        }
    }
}