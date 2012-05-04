using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Griffin.Networking.JsonRpc;
using Griffin.Networking.JsonRpc.Handlers;
using Griffin.Networking.JsonRpc.Remoting;
using Griffin.Networking.Logging;
using Griffin.Networking.Pipelines;

namespace ServerDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            LogManager.Assign(new SimpleLogManager<ConsoleLogger>());

            var factory = new DelegatePipelineFactory();
            //CreateSimplePipeline(factory);
            CreateRpcPipeline(factory);

            JsonRpcListener listener = new JsonRpcListener(factory);
            listener.Start(new IPEndPoint(IPAddress.Any, 3322));

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
