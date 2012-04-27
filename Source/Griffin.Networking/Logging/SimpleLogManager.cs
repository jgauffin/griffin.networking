using System;
using System.Linq.Expressions;

namespace Griffin.Networking.Logging
{
    /// <summary>
    /// Log manager which uses one of the base loggers.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimpleLogManager<T> : LogManager where T : BaseLogger
    {
        private readonly Func<Type, T> _factoryMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleLogManager{T}"/> class.
        /// </summary>
        public SimpleLogManager()
        {
            var constructor = typeof(T).GetConstructor(new Type[] {typeof (Type)});
            if (constructor == null)
                throw new ArgumentException("Must implement BaseLogger and have the same constructor signature.");


            var param = Expression.Parameter(typeof(Type), "type");
            var lambda = Expression.Lambda<Func<Type, T>>(Expression.New(constructor, param), param);
            _factoryMethod = lambda.Compile();
            
        }

        /// <summary>
        /// Get a logger for a type
        /// </summary>
        /// <param name="loggingType">Type that want's a logger</param>
        /// <returns>
        /// Logger
        /// </returns>
        protected override ILogger GetLoggerInternal(Type loggingType)
        {
            return _factoryMethod(loggingType);
        }
    }
}