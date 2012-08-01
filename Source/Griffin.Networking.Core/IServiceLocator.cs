using System;

namespace Griffin.Networking
{
    /// <summary>
    /// Used to build all <see cref="IPipelineHandler"/> classes.
    /// </summary>
    /// <remarks>
    /// <para>Important! It's up to your container to resolve the same instance of an class if it implements both <see cref="IUpstreamHandler"/> and <see cref="IDownstreamHandler"/> (and
    /// you need to use the same fields in both cases).
    /// </para>
    /// 
    /// </remarks>
    public interface IServiceLocator
    {
        /// <summary>
        /// Resolve a service
        /// </summary>
        /// <param name="type">Type of service to locate.</param>
        /// <returns>The registered service</returns>
        /// <exception cref="InvalidOperationException">Failed to find service.</exception>
        object Resolve(Type type);
    }
}