using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Griffin.Networking;
using Griffin.Networking.JsonRpc;
using Griffin.Networking.JsonRpc.Handlers;
using Griffin.Networking.JsonRpc.Messages;
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
            factory.AddUpstreamHandler(new HeaderDecoder());
            factory.AddUpstreamHandler(new BodyDecoder());
            factory.AddUpstreamHandler(new MyApplication());
            factory.AddDownstreamHandler(new ResponseEncoder());

            JsonRpcListener  listener = new JsonRpcListener(factory);
            listener.Start(new IPEndPoint(IPAddress.Any, 3322));

            Console.ReadLine();
        }
    }

    class MyApplication : IUpstreamHandler
    {
        /// <summary>
        /// Handle an message
        /// </summary>
        /// <param name="context">Context unique for this handler instance</param>
        /// <param name="message">Message to process</param>
        /// <remarks>
        /// All messages that can't be handled MUST be send up the chain using <see cref="IPipelineHandlerContext.SendUpstream"/>.
        /// </remarks>
        public void HandleUpstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            var msg = message as ReceivedRequest;
            if (msg != null)
            {
                Console.WriteLine("got request with id " + msg.Request.Id);
                var response = new Response(msg.Request.Id, "Hello world");
                context.SendDownstream(new SendResponse(response));
            }

        }
    }
}
