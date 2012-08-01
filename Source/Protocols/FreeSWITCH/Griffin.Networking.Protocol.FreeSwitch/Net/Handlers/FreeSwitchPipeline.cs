using System;
using System.IO;
using System.Security;
using Griffin.Networking.Pipelines;
using Griffin.Networking.Protocol.FreeSwitch.Events;

namespace Griffin.Networking.Protocol.FreeSwitch.Net.Handlers
{
    public class FreeSwitchPipeline : IPipelineFactory
    {
        private readonly IUpstreamHandler _client;
        private readonly SecureString _password;

        public FreeSwitchPipeline(SecureString password, IUpstreamHandler client)
        {
            _password = password;
            _client = client;
        }

        #region IPipelineFactory Members

        public IPipeline Build()
        {
            var pipeline = new Pipeline();
            //pipeline.Add(new ReconnectHandler(TimeSpan.FromSeconds(15)));
            var commandDispatcher = new CommandDispatcher();

            var logger =
                new ProtocolTrace(new FileStream(@"C:\temp\Str" + DateTime.Now.ToString("yyyy-MM-dd HHmmss") + ".log",
                                                 FileMode.Create));

            var eventFactory = new EventFactory();
            eventFactory.MapDefault();
            var eventDecoder = new EventDecoder(eventFactory);

            //channel will call this one.
            pipeline.AddUpstreamHandler(logger);
            pipeline.AddUpstreamHandler(new MessageDecoder());
            pipeline.AddUpstreamHandler(eventDecoder);
            pipeline.AddUpstreamHandler(commandDispatcher);
            pipeline.AddUpstreamHandler(new AuthenticationHandler(_password));
            pipeline.AddUpstreamHandler(_client);

            pipeline.AddDownstreamHandler(commandDispatcher);
            pipeline.AddDownstreamHandler(new MessageEncoder());
            pipeline.AddDownstreamHandler(logger);
            return pipeline;
        }

        #endregion

        public void AddLast<T>() where T : IPipelineHandler
        {
            throw new NotSupportedException();
        }

        public void AddLast(IPipelineHandler handler)
        {
        }
    }
}