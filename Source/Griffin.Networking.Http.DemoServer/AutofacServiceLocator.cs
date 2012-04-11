using System;
using Autofac;

namespace Griffin.Networking.Http.DemoServer
{
    public class AutofacServiceLocator : IServiceLocator, IScopeListener
    {
        private readonly IContainer _container;
        private ILifetimeScope _scope;

        public AutofacServiceLocator(IContainer container)
        {
            _container = container;
            RequestScope.Subscribe(this);
        }

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

        public void ScopeBegins()
        {
            _scope = _container.BeginLifetimeScope();
        }

        public void ScopeEnds()
        {
            _scope.Dispose();
        }
    }
}