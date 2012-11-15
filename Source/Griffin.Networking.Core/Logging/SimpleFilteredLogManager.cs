using System;
using System.Linq.Expressions;

namespace Griffin.Networking.Logging
{
    /// <summary>
    /// Logs all entries which has a minimum log level
    /// </summary>
    /// <typeparam name="T">Type of inner logger</typeparam>
    public class SimpleFilteredLogManager<T> : LogManager where T : BaseLogger
    {
        private readonly Func<Type, T> _factoryMethod;
        private readonly LogLevel _minimumLevel;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleFilteredLogManager{T}"/> class.
        /// </summary>
        public SimpleFilteredLogManager(LogLevel minimumLevel)
        {
            _minimumLevel = minimumLevel;
            var constructor = typeof (T).GetConstructor(new[] {typeof (LogLevel), typeof (Type), typeof (BaseLogger)});
            if (constructor == null)
                throw new ArgumentException("Must implement BaseLogger and have the same constructor signature.");


            var param = Expression.Parameter(typeof (Type), "type");
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
            var innerLogger = _factoryMethod(loggingType);
            return new FilteredLogger(_minimumLevel, loggingType, innerLogger);
        }
    }
}