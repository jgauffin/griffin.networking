using System;
using Griffin.Networking;

namespace ServerDemo
{
    public class SimpleServiceLocator : IServiceLocator
    {
        /// <summary>
        /// Resolve a service
        /// </summary>
        /// <param name="type">Type of service to locate.</param>
        // <param name="channel">Channel that the resolve is for. Can be used to control lifetime scope, as each handler should live as long as the channel.</param>
        /// <returns>The registered service</returns>
        /// <exception cref="InvalidOperationException">Failed to find service.</exception>
        public object Resolve(Type type)
        {
            return Activator.CreateInstance(type);
        }
    }
}