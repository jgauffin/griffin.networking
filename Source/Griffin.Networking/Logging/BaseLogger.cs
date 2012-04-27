using System;

namespace Griffin.Networking.Logging
{
    /// <summary>
    /// Base class for loggers.
    /// </summary>
    /// <remarks>All you have to do is to override <see cref="Write"/>.</remarks>
    public abstract class BaseLogger : ILogger
    {
        private readonly Type _loggedType;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseLogger"/> class.
        /// </summary>
        /// <param name="loggedType">Type being logged.</param>
        protected BaseLogger(Type loggedType)
        {
            _loggedType = loggedType;
        }

        /// <summary>
        /// Gets number of frames to skip when using stack trace
        /// </summary>
        protected virtual int SkipFrameCount
        {
            get { return 1; }
        }

        /// <summary>
        /// Gets the type for the class which logs using this class
        /// </summary>
        public Type LoggedType
        {
            get { return _loggedType; }
        }

        /// <summary>
        /// Detailed framework messages used to find wierd errors.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Trace(string message)
        {
            Write(LogLevel.Trace, message, null);
        }

        /// <summary>
        /// Detailed framework messages used to find wierd errors.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="exception">Thrown exception</param>
        public void Trace(string message, Exception exception)
        {
            Write(LogLevel.Trace, message, exception);
        }

        /// <summary>
        /// Diagonstic messages. Not as detailed as the trace messages but still only useful during debugging.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Debug(string message)
        {
            Write(LogLevel.Debug, message, null);
        }

        /// <summary>
        /// Diagonstic messages. Not as detailed as the trace messages but still only useful during debugging.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="exception">Exception which has been thrown</param>
        public void Debug(string message, Exception exception)
        {
            Write(LogLevel.Debug, message, exception);
        }

        /// <summary>
        /// Something did not go as planned, but the framework can still continue as expected.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Warning(string message)
        {
            Write(LogLevel.Warning, message, null);
        }

        /// <summary>
        /// Something did not go as planned, but the framework can still continue as expected.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="exception">Exception which has been thrown</param>
        public void Warning(string message, Exception exception)
        {
            Write(LogLevel.Warning, message, exception);
        }

        /// <summary>
        /// Something failed. The framework must abort the current processing
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Error(string message)
        {
            Write(LogLevel.Error, message, null);
        }

        /// <summary>
        /// Something failed. The framework must abort the current processing
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="exception">Exception which has been thrown</param>
        public void Error(string message, Exception exception)
        {
            Write(LogLevel.Error, message, exception);
        }

        /// <summary>
        /// Writes the specified log level.
        /// </summary>
        /// <param name="logLevel">Used level.</param>
        /// <param name="msg">Message.</param>
        /// <param name="exception">The exception (or null).</param>
        public abstract void Write(LogLevel logLevel, string msg, Exception exception);

        /// <summary>
        /// Formats exception details (including all inner exceptions)
        /// </summary>
        /// <param name="exception">Thrown exception.</param>
        /// <param name="spaces">Number of spaces to prefix each line with.</param>
        /// <returns>Formatted exception information</returns>
        /// <remarks>Increases the number of spaces for each inner exception so it's easy to see all information</remarks>
        protected virtual string BuildExceptionDetails(Exception exception, int spaces)
        {
            var buffer = "".PadLeft(spaces) + exception + "\r\n";
            if (exception.InnerException != null)
                buffer += BuildExceptionDetails(exception.InnerException, spaces + 4);

            return buffer;
        }
    }
}