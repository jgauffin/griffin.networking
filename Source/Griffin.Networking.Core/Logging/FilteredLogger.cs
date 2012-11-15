using System;

namespace Griffin.Networking.Logging
{
    /// <summary>
    /// Logger that filters on level
    /// </summary>
    public class FilteredLogger : BaseLogger
    {
        private readonly BaseLogger _innerLogger;
        private readonly LogLevel _minLevel;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteredLogger"/> class.
        /// </summary>
        /// <param name="type">Tyep that logs.</param>
        /// <param name="innerLogger">The inner logger (invoked if the filter requirements are fulfilled).</param>
        public FilteredLogger(Type type, BaseLogger innerLogger)
            : base(type)
        {
            if (innerLogger == null) throw new ArgumentNullException("innerLogger");
            _innerLogger = innerLogger;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="FilteredLogger"/> class.
        /// </summary>
        /// <param name="minLevel">The minimum level to log.</param>
        /// <param name="loggedType">Type being logged.</param>
        /// <param name="innerLogger">The inner logger (invoked if the filter requirements are fulfilled).</param>
        public FilteredLogger(LogLevel minLevel, Type loggedType, BaseLogger innerLogger) : base(loggedType)
        {
            if (innerLogger == null) throw new ArgumentNullException("innerLogger");
            _minLevel = minLevel;
            _innerLogger = innerLogger;
        }

        /// <summary>
        /// Gets number of frames to skip when using stack trace
        /// </summary>
        protected override int SkipFrameCount
        {
            get { return base.SkipFrameCount + 1; }
        }

        /// <summary>
        /// Logs an entry to the source
        /// </summary>
        /// <param name="logLevel">Used level.</param>
        /// <param name="msg">Message.</param>
        /// <param name="exception">The exception (or null).</param>
        public override void Write(LogLevel logLevel, string msg, Exception exception)
        {
            if (logLevel < _minLevel)
                return;

            _innerLogger.Write(logLevel, msg, exception);
        }
    }
}