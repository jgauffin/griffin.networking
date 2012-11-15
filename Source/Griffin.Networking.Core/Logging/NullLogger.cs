using System;

namespace Griffin.Networking.Logging
{
    /// <summary>
    /// Throws away all logs
    /// </summary>
    public class NullLogger : ILogger
    {
        #region ILogger Members

        /// <summary>
        /// Detailed framework messages used to find wierd errors.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Trace(string message)
        {
        }

        /// <summary>
        /// Detailed framework messages used to find wierd errors.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="exception">Thrown exception</param>
        public void Trace(string message, Exception exception)
        {
        }

        /// <summary>
        /// Diagonstic messages. Not as detailed as the trace messages but still only useful during debugging.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Debug(string message)
        {
        }

        /// <summary>
        /// Diagonstic messages. Not as detailed as the trace messages but still only useful during debugging.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="exception">Exception which has been thrown</param>
        public void Debug(string message, Exception exception)
        {
        }

        /// <summary>
        /// Something did not go as planned, but the framework can still continue as expected.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Warning(string message)
        {
        }

        /// <summary>
        /// Something did not go as planned, but the framework can still continue as expected.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="exception">Exception which has been thrown</param>
        public void Warning(string message, Exception exception)
        {
        }

        /// <summary>
        /// Something failed. The framework must abort the current processing
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Error(string message)
        {
        }

        /// <summary>
        /// Something failed. The framework must abort the current processing
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="exception">Exception which has been thrown</param>
        public void Error(string message, Exception exception)
        {
        }

        #endregion
    }
}