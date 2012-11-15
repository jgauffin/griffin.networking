using System;
using System.Collections.Generic;
using Autofac;
using Griffin.Networking.Http.Handlers;

namespace Griffin.Networking.Http.DemoServer.ThroughPipeline
{
    public class AutofacServiceLocator : IServiceLocator, IScopeListener
    {
        private readonly IContainer _container;
        private readonly Dictionary<object, ILifetimeScope> _scopes = new Dictionary<object, ILifetimeScope>();

        public AutofacServiceLocator(IContainer container)
        {
            _container = container;
        }

        #region IScopeListener Members

        public void ScopeStarted(object id)
        {
            _scopes[id] = _container.BeginLifetimeScope();
        }

        public void ScopeEnded(object id)
        {
            _scopes[id].Dispose();
            _scopes.Remove(id);
        }

        #endregion

        #region IServiceLocator Members

        /// <summary>
        /// Resolve a service
        /// </summary>
        /// <param name="type">Type of service to locate.</param>
        // <param name="channel">Channel that the resolve is for. Can be used to control lifetime scope, as each handler should live as long as the channel.</param>
        /// <returns>The registered service</returns>
        /// <exception cref="System.InvalidOperationException">Failed to find service.</exception>
        public object Resolve(Type type)
        {
            return _container.Resolve(type);
        }

        #endregion
    }
}