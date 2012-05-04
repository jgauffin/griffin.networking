using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.Networking.Pipelines
{
    /// <summary>
    /// Uses delegates to created scoped handlers
    /// </summary>
    public class DelegatePipelineFactory : IPipelineFactory
    {
         private readonly LinkedList<HandlerInformation<IUpstreamHandler>> _uptreamHandlers = new LinkedList<HandlerInformation<IUpstreamHandler>>();
        private readonly LinkedList<HandlerInformation<IDownstreamHandler>> _downstreamHandlers = new LinkedList<HandlerInformation<IDownstreamHandler>>();


        /// <summary>
        /// Add another handler.
        /// </summary>
        /// <param name="factoryMethod">The factory method.</param>
        public void AddDownstreamHandler(Func<IDownstreamHandler> factoryMethod)
        {
            _downstreamHandlers.AddLast(new HandlerInformation<IDownstreamHandler>(factoryMethod));
        }

        /// <summary>
        /// Add an handler instance (singleton)
        /// </summary>
        /// <param name="handler">Must implement <see cref="IDownstreamHandler"/> and/or <see cref="IUpstreamHandler"/></param>
        /// <remarks>Same instance will be used for all channels. Use the <see cref="IPipelineHandlerContext"/> to store any context information.</remarks>
        public void AddDownstreamHandler(IDownstreamHandler handler)
        {
            _downstreamHandlers.AddLast(new HandlerInformation<IDownstreamHandler>(handler));
        }

        /// <summary>
        /// Add another handler.
        /// </summary>
        /// <param name="factoryMethod">The factory method.</param>
        public void AddUpstreamHandler(Func<IUpstreamHandler> factoryMethod)
        {
            _uptreamHandlers.AddLast(new HandlerInformation<IUpstreamHandler>(factoryMethod));
        }

        /// <summary>
        /// Add an handler instance (singleton)
        /// </summary>
        /// <param name="handler">Must implement <see cref="IDownstreamHandler"/> and/or <see cref="IUpstreamHandler"/></param>
        /// <remarks>Same instance will be used for all channels. Use the <see cref="IPipelineHandlerContext"/> to store any context information.</remarks>
        public void AddUpstreamHandler(IUpstreamHandler handler)
        {
            _uptreamHandlers.AddLast(new HandlerInformation<IUpstreamHandler>(handler));
        }

        /// <summary>
        /// Create a pipeline for a channel
        /// </summary>
        /// <returns>Created pipeline</returns>
        public IPipeline Build()
        {
            var pipeline = new Pipeline();

            foreach (var handler in _uptreamHandlers)
            {
                if (handler.Factory != null)
                    pipeline.AddUpstreamHandler((IUpstreamHandler) handler.Factory());
                else
                    pipeline.AddUpstreamHandler(handler.Handler);
            }
            foreach (var handler in _downstreamHandlers)
            {
                if (handler.Factory != null)
                    pipeline.AddDownstreamHandler((IDownstreamHandler)handler.Factory());
                else
                    pipeline.AddDownstreamHandler(handler.Handler);
            }
            return pipeline;
        }

        #region Nested type: Wrapper

        private class HandlerInformation<T>
        {
            public HandlerInformation(Func<T> factory)
            {
                Factory = factory;
            }

            public HandlerInformation(T handler)
            {
                Handler = handler;
            }

            public T Handler { get; private set; }

            public Func<T> Factory { get; private set; }
        }

        #endregion
    }
}
