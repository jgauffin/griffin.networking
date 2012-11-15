using System;
using System.Net;
using Griffin.Networking.JsonRpc.Handlers;
using Griffin.Networking.JsonRpc.Remoting;
using Griffin.Networking.Logging;
using Griffin.Networking.Pipelines;
using Griffin.Networking.Servers;

namespace ServerDemo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            LogManager.Assign(new SimpleLogManager<ConsoleLogger>());

            var factory = new DelegatePipelineFactory();
            //CreateSimplePipeline(factory);
            CreateRpcPipeline(factory);

            var server = new PipelineServer(factory, new ServerConfiguration());
            server.Start(new IPEndPoint(IPAddress.Any, 3322));

            Console.ReadLine();
        }


        private static void CreateRpcPipeline(DelegatePipelineFactory factory)
        {
            var invoker = new RpcServiceInvoker(new DotNetValueConverter(), new SimpleServiceLocator());
            invoker.Map<MathModule>();

            factory.AddUpstreamHandler(() => new HeaderDecoder());
            factory.AddUpstreamHandler(() => new BodyDecoder());
            factory.AddUpstreamHandler(new RequestHandler(invoker));
            factory.AddDownstreamHandler(new ResponseEncoder());
        }


        private static void CreateSimplePipeline(DelegatePipelineFactory factory)
        {
            factory.AddUpstreamHandler(() => new HeaderDecoder());
            factory.AddUpstreamHandler(() => new BodyDecoder());
            factory.AddUpstreamHandler(new MyApplication());
            factory.AddDownstreamHandler(new ResponseEncoder());
        }
    }
}