namespace Griffin.Networking.Pipelines
{
    /// <summary>
    /// Context for a downstream (from channel to client) handler
    /// </summary>
    class PipelineDownstreamContext : IPipelineHandlerContext
    {
        private readonly IPipeline _pipeline;
        private readonly IDownstreamHandler _myHandler;
        private PipelineDownstreamContext _nextHandler;
        private ILogger _logger = LogManager.GetLogger<PipelineDownstreamContext>();

        public PipelineDownstreamContext(IPipeline pipeline, IDownstreamHandler myHandler)
        {
            _pipeline = pipeline;
            _myHandler = myHandler;
        }

        public void SendDownstream(IPipelineMessage message)
        {
            if (_nextHandler != null)
            {
                _logger.Trace("Down: " + _myHandler.ToStringOrClassName() + " is passing on message");
                _nextHandler.Invoke(message);
            }
            else
            {
                _logger.Warning("Down: " +_myHandler.ToStringOrClassName() + " tried to send message, but there are no more handlers.");
            }
        }

        public PipelineDownstreamContext NextHandler
        {
            set { _nextHandler = value; }
        }


        public void Invoke(IPipelineMessage message)
        {
            _logger.Trace("Down: Invoking " + _myHandler.ToStringOrClassName() + " with msg " + message.ToStringOrClassName());
            _myHandler.HandleDownstream(this, message);
        }


        public void SendUpstream(IPipelineMessage message)
        {
            _logger.Trace("Up: " + _myHandler.ToStringOrClassName() + " is sending " + message.ToStringOrClassName());
            _pipeline.SendUpstream(message);
        }

        public override string ToString()
        {
            return _myHandler.ToString();
        }
    }
}