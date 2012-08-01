using System;

namespace Griffin.Networking.Logging
{
    /// <summary>
    /// Logging interface
    /// </summary>
    /// <remarks>You typically just want to log the warnings and the errors from the framework since your logs
    /// will probably be filled very quickly otherwise.</remarks>
    public interface ILogger
    {
        /// <summary>
        /// Detailed framework messages used to find wierd errors.
        /// </summary>
        /// <param name="message">Message to log</param>
        void Trace(string message);


        /// <summary>
        /// Detailed framework messages used to find wierd errors.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="exception">Thrown exception</param>
        void Trace(string message, Exception exception);

        /// <summary>
        /// Diagonstic messages. Not as detailed as the trace messages but still only useful during debugging.
        /// </summary>
        /// <param name="message">Message to log</param>
        void Debug(string message);

        /// <summary>
        /// Diagonstic messages. Not as detailed as the trace messages but still only useful during debugging.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="exception">Exception which has been thrown</param>
        void Debug(string message, Exception exception);

        /// <summary>
        /// Something did not go as planned, but the framework can still continue as expected.
        /// </summary>
        /// <param name="message">Message to log</param>
        void Warning(string message);


        /// <summary>
        /// Something did not go as planned, but the framework can still continue as expected.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="exception">Exception which has been thrown</param>
        void Warning(string message, Exception exception);

        /// <summary>
        /// Something failed. The framework must abort the current processing
        /// </summary>
        /// <param name="message">Message to log</param>
        void Error(string message);

        /// <summary>
        /// Something failed. The framework must abort the current processing
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="exception">Exception which has been thrown</param>
        void Error(string message, Exception exception);
    }
}