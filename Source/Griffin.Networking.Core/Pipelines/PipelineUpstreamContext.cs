using Griffin.Networking.Logging;

namespace Griffin.Networking.Pipelines
{
    /// <summary>
    /// Context for downstream handlers.
    /// </summary>
    /// <remarks>Each context is unique for a handler in a channel.</remarks>
    internal class PipelineUpstreamContext : IPipelineHandlerContext
    {
        private readonly ILogger _logger = LogManager.GetLogger<PipelineUpstreamContext>();
        private readonly IUpstreamHandler _myHandler;
        private readonly IPipeline _pipeline;
        private PipelineUpstreamContext _nextHandler;

        public PipelineUpstreamContext(IPipeline pipeline, IUpstreamHandler myHandler)
        {
            _pipeline = pipeline;
            _myHandler = myHandler;
        }

        public PipelineUpstreamContext NextHandler
        {
            set { _nextHandler = value; }
        }

        #region IPipelineHandlerContext Members

        public void SendUpstream(IPipelineMessage message)
        {
            if (_nextHandler != null)
            {
                _logger.Trace("Up: " + _myHandler.ToStringOrClassName() + " sends message " +
                              message.ToStringOrClassName());
                _nextHandler.Invoke(message);
            }
            else
            {
                _logger.Warning("Up: " + _myHandler.ToStringOrClassName() + " tried to send message " +
                                message.ToStringOrClassName() + ", but there are no handler upstream.");
            }
        }


        public void SendDownstream(IPipelineMessage message)
        {
            _logger.Trace("Down: " + _myHandler.ToStringOrClassName() + " sends " + message.ToStringOrClassName());
            _pipeline.SendDownstream(message);
        }

        #endregion

        public void Invoke(IPipelineMessage message)
        {
            _myHandler.HandleUpstream(this, message);
        }

        public override string ToString()
        {
            return _myHandler.ToString();
        }
    }
}